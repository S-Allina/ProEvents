using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Identity.Interfeces;
using ProEvent.Services.Identity.Models;

namespace WebApp.Controllers
{

    [ApiController]
    [Route("/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public AuthController(IAuthenticationService authenticationService, UserManager<ApplicationUser> userManager)
        {
            _authenticationService = authenticationService;
            _userManager= userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationModel model)
        {
            var result = await _authenticationService.RegisterUser(model);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Registration failed.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            try
            {
                var result = await _authenticationService.LoginUser(model);

                if (result != null)
                {
                    
                    return Ok(result);
                }
                else
                {
                    return Unauthorized(new { displayMessage = "Invalid login attempt." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Unauthorized(new { displayMessage = ex.Message });
            }
        }



        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authenticationService.LogoutUser();
            return Ok();
        }
    }
}