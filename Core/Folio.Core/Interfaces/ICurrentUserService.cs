using Folio.Core.Domain;

namespace Folio.Core.Interfaces
{
    public interface ICurrentUserService
    {
        int GetCurrentUserId();
        string GetCurrentUserEmail();
        Task<User?> GetCurrentUserAsync();
    }
}
