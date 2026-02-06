using System.ComponentModel.DataAnnotations;

namespace Folio.Core.Application.DTOs.Auth
{
    public class LoginCredentialsDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress(ErrorMessage = "The field {0} must be a valid email address")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        public required string Password { get; set; }
    }
}
