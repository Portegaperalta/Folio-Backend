using Folio.Core.Domain;

namespace Folio.Core.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateJwt(User userEntity);
    }
}
