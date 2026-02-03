using Folio.Core.Domain.Entities;
using FolioWebAPI.DTOs.Bookmark;

namespace FolioWebAPI.Mappers
{
    public class BookmarkMapper
    {
        public BookmarkDTO ToDto(Bookmark bookmarkEntity)
        {
            return new BookmarkDTO
            {
                Id = bookmarkEntity.Id,
                Name = bookmarkEntity.Name,
                Url = bookmarkEntity.Url,
                IsMarkedFavorite = bookmarkEntity.IsMarkedFavorite,
                CreationDate = bookmarkEntity.CreationDate,
                LastVisitedTime = bookmarkEntity.LastVisitedTime
            };
        }

        public Bookmark ToEntity(Guid userId,BookmarkCreationDTO bookmarkCreationDTO)
        {
            return new Bookmark(bookmarkCreationDTO.Name, 
                                bookmarkCreationDTO.Url,
                                bookmarkCreationDTO.FolderId,
                                userId);
        }
    }
}
