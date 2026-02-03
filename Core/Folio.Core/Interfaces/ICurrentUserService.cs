using Folio.Core.Domain.Entities;

namespace Folio.Core.Interfaces
{
    public interface ICurrentUserService
    {
        Guid GetCurrentUserId();
        string GetCurrentUserEmail();
        Task<User?> GetCurrentUserAsync();
    }
}
