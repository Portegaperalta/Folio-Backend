using Folio.Core.Application.DTOs.Folder;
using Folio.Core.Application.DTOs.Pagination;
using Folio.Core.Application.Mappers;
using Folio.Core.Domain.Exceptions.Folder;
using Folio.Core.Interfaces;

namespace Folio.Core.Application.Services
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly FolderMapper _folderMapper;
        private readonly ICacheService _cacheService;
        private readonly TimeSpan cacheDuration = TimeSpan.FromMinutes(5);

        public FolderService(IFolderRepository folderRepository, FolderMapper folderMapper, ICacheService cacheService)
        {
            _folderRepository = folderRepository;
            _folderMapper = folderMapper;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<FolderDTO>> GetAllFoldersDTOsAsync(Guid userId, PaginationDTO paginationDTO)
        {
            var versionKey = $"folio:folders:{userId}:v";
            var version = await _cacheService.GetAsync<long?>(versionKey) ?? 1;
            var cacheKey = $"folio:folders:{userId}:p{paginationDTO.Page}:r:{paginationDTO.RecordsPerPage}:v{version}";

            var cached = await _cacheService.GetAsync<List<FolderDTO>>(cacheKey);

            if (cached is not null)
                return cached;

            var folders = await _folderRepository.GetAllAsync(userId, paginationDTO);

            var foldersDTOs = folders.Select(f => _folderMapper.ToDto(f));

            await _cacheService.SetAsync(cacheKey, foldersDTOs, cacheDuration);

            return foldersDTOs;
        }

        public async Task<FolderDTO?> GetFolderDTOByIdAsync(Guid userId, Guid folderId)
        {
            var versionKey = $"folio:folders:{userId}:v";
            var version = await _cacheService.GetAsync<long?>(versionKey) ?? 1;

            var cacheKey = $"folio:folders:{userId}:byid:{folderId}:v{version}";
            var cached = await _cacheService.GetAsync<FolderDTO>(cacheKey);
            if (cached is not null)
                return cached;

            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
                return null;

            var folderDTO = _folderMapper.ToDto(folder);

            await _cacheService.SetAsync(cacheKey, folderDTO, cacheDuration);

            return folderDTO;
        }

        public async Task<int> CountFoldersAsync(Guid userId)
        {
            var versionKey = $"folio:folders:{userId}:v";
            var version = await _cacheService.GetAsync<long?>(versionKey) ?? 1;
            var cacheKey = $"folio:folders:{userId}:count:v{version}";

            var cached = await _cacheService.GetAsync<int?>(cacheKey);

            if (cached.HasValue)
                return cached.Value;

            var folderCount = await _folderRepository.CountByUserAsync(userId);

            await _cacheService.SetAsync(cacheKey, folderCount, cacheDuration);

            return folderCount;
        }

        public async Task<FolderDTO> CreateFolderAsync(Guid userId, FolderCreationDTO folderCreationDTO)
        {
            ArgumentNullException.ThrowIfNull(folderCreationDTO);

            var folderEntity = _folderMapper.ToEntity(userId, folderCreationDTO);

            await _folderRepository.AddAsync(folderEntity);

            await _cacheService.IncrementAsync($"folio:folders:{userId}:v");

            var folderDTO = _folderMapper.ToDto(folderEntity);

            return folderDTO;
        }

        public async Task UpdateFolderAsync(Guid folderId, Guid userId, FolderUpdateDTO folderUpdateDTO)
        {
            var folderEntity = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folderEntity is null)
            {
                throw new FolderNotFoundException(folderId);
            }

            if (folderUpdateDTO.Name is not null)
            {
                folderEntity.ChangeName(folderUpdateDTO.Name);
            }

            if (folderUpdateDTO.IsMarkedFavorite.HasValue)
            {
                if (folderUpdateDTO.IsMarkedFavorite is true)
                {
                    folderEntity.MarkFavorite();
                }
                else
                {
                    folderEntity.UnmarkFavorite();
                }
            }

            await _folderRepository.UpdateAsync(folderEntity);

            await _cacheService.IncrementAsync($"folio:folders:{userId}:v");
        }

        public async Task MarkFolderAsVisitedAsync(Guid userId, Guid folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folder is null)
                throw new FolderNotFoundException(folderId);

            folder.Visit();

            await _folderRepository.UpdateAsync(folder);

            await _cacheService.IncrementAsync($"folio:folders:{userId}:v");
        }

        public async Task<bool> DeleteFolderAsync(Guid userId, Guid folderId)
        {
            var folderEntity = await _folderRepository.GetByIdAsync(userId, folderId);

            if (folderEntity is null)
            {
                return false;
            }

            await _folderRepository.DeleteAsync(folderEntity);

            await _cacheService.IncrementAsync($"folio:folders:{userId}:v");

            return true;
        }
    }
}
