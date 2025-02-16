using ProEvent.Services.Identity.DTOs;
using ProEvent.Services.Identity.Models;


namespace ProEvent.Services.Identity.Interfeces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponseDTO> RegisterUser(UserRegistrationModel model, CancellationToken cancellationToken);
        Task<AuthenticationResponseDTO> LoginUser(UserLoginModel model, CancellationToken cancellationToken);
        Task<bool> LogoutUser(CancellationToken cancellationToken);
    }
}
