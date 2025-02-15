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
        public async Task<IActionResult> Register(UserRegistrationModel model)
        {
            AuthenticationResponseDTO response = new AuthenticationResponseDTO();
            try
            {
                AuthenticationResponseDTO result = await _authenticationService.RegisterUser(model);
                response.Token = result.Token;
                response.UserId = result.UserId;
                response.UserName = result.UserName;
                response.Email = result.Email;
                response.Role = result.Role;
                response.DisplayMessage = "Пользователь успешно зарегистрирован.";
                response.IsSuccess = true;
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                response.IsSuccess = false;
                response.DisplayMessage = "Регистрация не удалась.";
                response.ErrorMessages = new List<string?> { ex.Message };
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.DisplayMessage = "Внутренняя ошибка сервера во время регистрации.";
                response.ErrorMessages = new List<string?> { ex.Message };

                return StatusCode(500, response);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            AuthenticationResponseDTO response = new AuthenticationResponseDTO();
            try
            {
                AuthenticationResponseDTO result = await _authenticationService.LoginUser(model);
                if (result != null)
                {
                    response.Token = result.Token;
                    response.UserId = result.UserId;
                    response.UserName = result.UserName;
                    response.Email = result.Email;
                    response.Role = result.Role;
                    response.DisplayMessage = "Вход успешен.";
                    response.IsSuccess = true;
                    return Ok(response);
                }
                else
                {
                    response.IsSuccess = false;
                    response.DisplayMessage = "Неверная попытка входа.";
                    response.ErrorMessages = new List<string?> { "Неверное имя пользователя или пароль." };
                    return Unauthorized(response);
                }
            }
            catch (ArgumentException ex)
            {
                response.IsSuccess = false;
                response.DisplayMessage = "Ошибка входа.";
                response.ErrorMessages = new List<string?> { ex.Message };
                return Unauthorized(response); 
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.DisplayMessage = "Внутренняя ошибка сервера при входе в систему.";
                response.ErrorMessages = new List<string?> { "Произошла непредвиденная ошибка." };
                return StatusCode(500, response);
            }
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            AuthenticationResponseDTO response = new AuthenticationResponseDTO(); 
            try
            {
                await _authenticationService.LogoutUser();
                response.IsSuccess = true;
                response.DisplayMessage = "Выход был успешен.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.DisplayMessage = "Ошибка выхода.";
                response.ErrorMessages = new List<string?> { ex.Message }; 
                return StatusCode(500, response);
            }
        }
    }
}