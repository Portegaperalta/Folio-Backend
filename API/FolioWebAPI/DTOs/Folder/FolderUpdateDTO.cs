using System.ComponentModel.DataAnnotations;

namespace FolioWebAPI.DTOs.Folder
{
    public class FolderUpdateDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 150, MinimumLength = 1, ErrorMessage = "The field {0} must have {1} characters or less")]
        public string? Name { get; set; }
        public bool? IsMarkedFavorite { get; set; }
    }
}
