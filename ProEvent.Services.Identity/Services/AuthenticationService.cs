using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Identity.DTOs;
using ProEvent.Services.Identity.Interfeces;
using ProEvent.Services.Identity.Models;
using ProEvent.Services.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProEvent.Services.Identity.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _dbContext;  // Inject ApplicationDbContext
        private readonly IValidator<Participant> _participantValidator;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            ApplicationDbContext dbContext,
            IValidator<Participant> participantValidator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _dbContext = dbContext;
            _participantValidator = participantValidator;
        }

        public async Task<AuthenticationResponseDTO> RegisterUser(UserRegistrationModel model)
        {
            var existingUserWithEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserWithEmail != null)
            {
                throw new ArgumentException("Пользователь с таким email уже зарегистрирован.");
            }
            var existingUserWithUsername = await _userManager.FindByNameAsync(model.UserName);
            if (existingUserWithUsername != null)
            {
                throw new ArgumentException("Логин пользователя не уникален.");
            }

            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var participant = new Participant
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    Email = model.Email,
                    UserId = user.Id
                };

                var participantValidationResult = await _participantValidator.ValidateAsync(participant);
                if (!participantValidationResult.IsValid)
                {
                    await _userManager.DeleteAsync(user);
                    var errors = participantValidationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    throw new ArgumentException(string.Join(" ", errors));
                }

                _dbContext.Participants.Add(participant);
                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await _userManager.DeleteAsync(user);
                    throw new Exception("Ошибка при регистрации.", ex);
                }

                await _userManager.AddToRoleAsync(user, "User");
                return await GenerateAuthenticationResponse(user);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Ошибки регистрации: {errors}");
            }
        }

        public async Task<AuthenticationResponseDTO> LoginUser(UserLoginModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Неверный формат данных.");
            }

            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
            {
                throw new ArgumentException("Требуется имя пользователя и пароль.");
            }

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    throw new Exception("Пользователь не найден после успешного входа в систему.");
                }

                return await GenerateAuthenticationResponse(user);
            }
            else
            {
                throw new Exception("Пользователя не существует.");
            }
        }
        public async Task<bool> LogoutUser()
        {
            await _signInManager.SignOutAsync();
            return true;
        }
        private async Task<AuthenticationResponseDTO> GenerateAuthenticationResponse(ApplicationUser user)
        {
            var token = _tokenService.GenerateToken(user);
            var role = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            var roleString = role.FirstOrDefault();

            return new AuthenticationResponseDTO
            {
                Token = token,
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = roleString
            };
        }
    }
}
