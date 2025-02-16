using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Identity.DTOs;
using ProEvent.Services.Identity.Interfeces;
using ProEvent.Services.Identity.Models;

namespace WebApp.Controllers
{

    [ApiController]
    [Route("/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationModel model, CancellationToken cancellationToken)
        {
            AuthenticationResponseDTO result = await _authenticationService.RegisterUser(model, cancellationToken);
            result.DisplayMessage = "Пользователь успешно зарегистрирован.";
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginModel model, CancellationToken cancellationToken)
        {
            AuthenticationResponseDTO result = await _authenticationService.LoginUser(model, cancellationToken);

            if (result == null)
            {
                result.IsSuccess = false;
                result.DisplayMessage = "Неверная попытка входа.";
                return Unauthorized(result);
            }

            result.DisplayMessage = "Вход успешен.";

            return Ok(result);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            AuthenticationResponseDTO response = new AuthenticationResponseDTO();
            await _authenticationService.LogoutUser(cancellationToken);
            response.DisplayMessage = "Выход был успешен.";
            return Ok(response);
        }
    }
}