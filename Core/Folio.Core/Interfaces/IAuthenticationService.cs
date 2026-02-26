using Folio.Core.Application.DTOs.Auth;
using Folio.Core.Domain.Entities;

namespace Folio.Core.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponseDTO> RegisterAsync(string name, string email, string password, string? phoneNumber);
        Task<AuthenticationResponseDTO?> LoginAsync(LoginCredentialsDTO loginCredentialsDTO);
        AuthenticationResponseDTO RenewToken(User userEntity);
    }
}
