using System.ComponentModel.DataAnnotations;

namespace Folio.Core.Application.DTOs.Bookmark
{
    public class BookmarkCreationDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Url(ErrorMessage = "The field {0} must be a valid Url")]
        public required string Url { get; set; }
    }
}
