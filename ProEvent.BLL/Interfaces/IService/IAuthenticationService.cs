

using ProEvent.BLL.DTOs;
using ProEvent.Domain.Models;

namespace ProEvent.BLL.Interfaces.IService
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponseDTO> RegisterUser(UserRegistrationModel model, CancellationToken cancellationToken);
        Task<AuthenticationResponseDTO> LoginUser(UserLoginModel model, CancellationToken cancellationToken);
        Task<bool> LogoutUser(CancellationToken cancellationToken);
    }
}
