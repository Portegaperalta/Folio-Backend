namespace Folio.Infrastructure.Identity
{
    public class JwtOptions
    {
        public string Issuer { get; init; } = null!;
        public string Audience { get; init; } = null!;
        public string SecretKey { get; init; } = null!;
        public int ExpiryMinutes { get; init; }
    }
}
