using Folio.Core.Application.DTOs.Folder;

namespace Folio.Core.Interfaces
{
    public interface IFolderService
    {
        Task<IEnumerable<FolderDTO>> GetAllFoldersDTOsAsync(Guid userId);
        Task<FolderDTO?> GetFolderDTOByIdAsync(Guid userId, Guid folderId);
        Task<int> CountFoldersAsync(Guid userId);
        Task<FolderDTO> CreateFolderAsync(Guid userId, FolderCreationDTO folderCreationDTO);
        Task UpdateFolderAsync(Guid folderId, Guid userId, FolderUpdateDTO folderUpdateDTO);
        Task MarkFolderAsVisitedAsync(Guid userId, Guid folderId);
        Task<bool> DeleteFolderAsync(Guid userId, Guid folderId);
    }
}
