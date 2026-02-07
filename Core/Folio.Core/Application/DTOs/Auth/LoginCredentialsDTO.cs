using System.ComponentModel.DataAnnotations;

namespace Folio.Core.Application.DTOs.Auth
{
    public class LoginCredentialsDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress(ErrorMessage = "The field {0} must be a valid email address")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MinLength(8, ErrorMessage = "The field {0} must be at least 8 characters long")]
        public required string Password { get; set; }
    }
}
