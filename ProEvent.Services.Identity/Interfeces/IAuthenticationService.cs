using ProEvent.Services.Identity.DTOs;
using ProEvent.Services.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProEvent.Services.Identity.Interfeces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponseDTO> RegisterUser(UserRegistrationModel model);
        Task<AuthenticationResponseDTO> LoginUser(UserLoginModel model);
        Task<bool> LogoutUser(); // Если нужна поддержка logout (например, invalidation JWT token)
    }
}
