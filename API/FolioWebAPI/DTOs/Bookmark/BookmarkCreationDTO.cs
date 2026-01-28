using System.ComponentModel.DataAnnotations;

namespace FolioWebAPI.DTOs.Bookmark
{
    public class BookmarkCreationDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(150, ErrorMessage = "The field {0} must have {1} characters or less")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Url(ErrorMessage = "The field {0} must be a valid Url")]
        public required string Url { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        public required int FolderId { get; set; }
    }
}
