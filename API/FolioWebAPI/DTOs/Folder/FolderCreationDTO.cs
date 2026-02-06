using System.ComponentModel.DataAnnotations;

namespace FolioWebAPI.DTOs.Folder
{
    public class FolderCreationDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 150, MinimumLength = 1, ErrorMessage = "The length of field {0} must be between {2} and {1}")]
        public required string Name { get; set; }
    }
}
