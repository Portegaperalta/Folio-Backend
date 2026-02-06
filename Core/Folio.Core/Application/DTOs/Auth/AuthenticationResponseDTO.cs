namespace FolioWebAPI.DTOs.Auth
{
    public class AuthenticationResponseDTO
    {
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
