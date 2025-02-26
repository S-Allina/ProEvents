using Microsoft.AspNetCore.Identity;
using ProEvent.Domain.Models;

namespace ProEvent.DAL.Interfaces.IRepository
{
    public interface IAuthenticationRepository
    {
        Task<ApplicationUser> FindUserByNameAsync(string userName, CancellationToken cancellationToken);
        Task<ApplicationUser> FindUserByEmailAsync(string email, CancellationToken cancellationToken);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
        Task SignOutAsync();
        Task AddToRoleAsync(ApplicationUser user, string role);
        Task DeleteUserAsync(ApplicationUser user);
        Task<string> GetRoleAsync(ApplicationUser user);
    }
}
