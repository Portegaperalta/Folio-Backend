using Folio.Core.Domain;

namespace Folio.Core.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User> RegisterAsync(string name, string email, string password);
        Task<User?> LoginAsync(string email, string password);
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
