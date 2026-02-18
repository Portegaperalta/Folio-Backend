using System.ComponentModel.DataAnnotations;

namespace Folio.Core.Application.DTOs.Folder
{
    public class FolderCreationDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        public required string Name { get; set; }
    }
}
