using FolioWebAPI.DTOs.Folder;
using Folio.Core.Domain;

namespace FolioWebAPI.Mappers
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
