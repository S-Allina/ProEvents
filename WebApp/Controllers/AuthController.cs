using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProEvent.BLL.DTOs;
using ProEvent.BLL.Interfaces.IService;
using ProEvent.Domain.Models;

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