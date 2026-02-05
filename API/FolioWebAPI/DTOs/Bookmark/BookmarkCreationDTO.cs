using System.ComponentModel.DataAnnotations;

namespace FolioWebAPI.DTOs.Bookmark
{
    public class BookmarkCreationDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 150, MinimumLength = 1, ErrorMessage = "The length of field {0} must be between {2} and {1}")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Url(ErrorMessage = "The field {0} must be a valid Url")]
        public required string Url { get; set; }
    }
}
