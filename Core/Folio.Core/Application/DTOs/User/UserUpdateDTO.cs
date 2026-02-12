using System.ComponentModel.DataAnnotations;

namespace Folio.Core.Application.DTOs.User
{
    public class UserUpdateDTO
    {
        public required Guid UserId { get; set; }

        [MaxLength(250, ErrorMessage = "The field {0} must have {1} characters or less")]
        [MinLength(1, ErrorMessage = "The field {0} must have {0} or more")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress(ErrorMessage = "The field {0} must be a valid email address")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "The field {0} must be a valid phone number")]
        public string? PhoneNumber { get; set; }
    }
}
