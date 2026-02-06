using Folio.Core.Domain.Entities;
using Folio.Core.Application.DTOs.Folder;

namespace Folio.Core.Application.Mappers
{
    public class FolderMapper
    {
        public FolderDTO ToDto(Folder folderEntity)
        {
            return new FolderDTO
            {
                Id = folderEntity.Id,
                Name = folderEntity.Name,
                IsMarkedFavorite = folderEntity.IsMarkedFavorite,
                CreationDate = folderEntity.CreationDate,
                LastVisitedTime = folderEntity.LastVisitedTime
            };
        }

        public Folder ToEntity(Guid userId, FolderCreationDTO folderCreationDTO)
        {
            return new Folder(folderCreationDTO.Name, userId);
        }
    }
}
