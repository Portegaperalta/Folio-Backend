using Folio.Core.Application.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Folio.Core.Application.DTOs.Auth
{
    public class RegisterCredentialsDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(250, ErrorMessage = "The field {0} must have {1} characters or less")]
        [MinLength(1, ErrorMessage = "The field {0} must have {0} or more")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress(ErrorMessage = "The field {0} must be a valid email address")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MinLength(8, ErrorMessage = "The field {0} must be at least {1} characters long")]
        [SpecialCharacter]
        public required string Password { get; set; }

        [Phone(ErrorMessage = "The field {0} must be a valid phone number")]
        public string? PhoneNumber { get; set; }
    }
}
