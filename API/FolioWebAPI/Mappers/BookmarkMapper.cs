using Folio.Core.Application.DTOs.Bookmark;
using Folio.Core.Domain.Entities;

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

        public Bookmark ToEntity(Guid userId, Guid folderId, BookmarkCreationDTO bookmarkCreationDTO)
        {
            return new Bookmark(bookmarkCreationDTO.Name, 
                                bookmarkCreationDTO.Url,
                                folderId,
                                userId);
        }
    }
}
