using Folio.Core.Application.DTOs.Folder;
using Folio.Core.Application.DTOs.Pagination;

namespace Folio.Core.Interfaces
{
    public interface IFolderService
    {
        Task<IEnumerable<FolderDTO>> GetAllFoldersDTOsAsync(Guid userId, PaginationDTO paginationDTO);
        Task<FolderDTO?> GetFolderDTOByIdAsync(Guid userId, Guid folderId);
        Task<int> CountFoldersAsync(Guid userId);
        Task<FolderDTO> CreateFolderAsync(Guid userId, FolderCreationDTO folderCreationDTO);
        Task UpdateFolderAsync(Guid folderId, Guid userId, FolderUpdateDTO folderUpdateDTO);
        Task MarkFolderAsVisitedAsync(Guid userId, Guid folderId);
        Task<bool> DeleteFolderAsync(Guid userId, Guid folderId);
    }
}
