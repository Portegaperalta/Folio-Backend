using System.ComponentModel.DataAnnotations;

namespace Folio.Core.Application.DTOs.Bookmark
{
    public class BookmarkUpdateDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        public Guid Id { get; set; }

        [StringLength(maximumLength: 150, MinimumLength = 1, ErrorMessage = "The length of field {0} must be between {2} and {1}")]
        public string? Name { get; set; }

        [Url(ErrorMessage = "The field {0} must be a valid Url")]
        public string? Url { get; set; }
        public bool? IsMarkedFavorite { get; set; }
    }
}
