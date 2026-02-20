namespace Folio.Core.Application.DTOs.User
{
    public class UserProfileDetailsDTO
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
