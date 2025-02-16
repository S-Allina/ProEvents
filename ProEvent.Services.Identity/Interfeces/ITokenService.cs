using ProEvent.Services.Core.Models;


namespace ProEvent.Services.Identity.Interfeces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
    }
}
