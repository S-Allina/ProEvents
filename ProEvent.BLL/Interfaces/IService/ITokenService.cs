
using ProEvent.Domain.Models;

namespace ProEvent.BLL.Interfaces.IService
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
    }
}
