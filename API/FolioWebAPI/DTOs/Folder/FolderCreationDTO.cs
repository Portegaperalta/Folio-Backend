using System.ComponentModel.DataAnnotations;

namespace FolioWebAPI.DTOs.Folder
{
    public class FolderCreationDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(150, ErrorMessage = "The field {0} must have {1} characters or less")]
        public required string Name { get; set; }
    }
}
