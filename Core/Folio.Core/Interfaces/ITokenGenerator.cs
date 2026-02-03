using Folio.Core.Domain.Entities;

namespace Folio.Core.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateJwt(User userEntity);
    }
}
