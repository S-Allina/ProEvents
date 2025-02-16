using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Identity.Interfeces;
using ProEvent.Services.Identity.Repository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProEvent.Services.Identity.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly IAuthenticationRepository _authenticationRepository;
        public TokenService(IConfiguration configuration, IAuthenticationRepository authenticationRepository)
        {
            _configuration = configuration;
            _jwtSecret = _configuration["Jwt:Secret"];
            _jwtIssuer = _configuration["Jwt:Issuer"];
            _jwtAudience = _configuration["Jwt:Audience"];
            _authenticationRepository = authenticationRepository;
        }

        public async Task<string> GenerateToken(ApplicationUser user)
        {
            var claims = new List<Claim>
 {
 new Claim(JwtRegisteredClaimNames.Sub, user.Id),
 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
 new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userName", user.UserName),
 };
            string role = await _authenticationRepository.GetRoleAsync(user); // Исправлено: await

            // Добавляем claim для роли
            claims.Add(new Claim("role", role));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


}
