using Folio.Core.Domain;

namespace Folio.Core.Interfaces
{
    public interface ICurrentUserService
    {
        Guid GetCurrentUserId();
        string GetCurrentUserEmail();
        Task<User?> GetCurrentUserAsync();
    }
}
