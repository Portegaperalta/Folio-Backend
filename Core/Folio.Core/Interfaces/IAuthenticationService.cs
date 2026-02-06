using Folio.Core.Application.DTOs.Auth;
using Folio.Core.Domain.Entities;

namespace Folio.Core.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponseDTO> RegisterAsync(string name, string email, string password);
        Task<User?> LoginAsync(string email, string password);
    }
}
