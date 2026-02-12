namespace Folio.Core.Application.DTOs.Folder
{
    public record FolderPaginationDTO(int Page = 1, int RecordsPerPage = 20)
    {
        private const int MaxRecordsPerPage = 50;

        public int Page { get; init; } = Math.Max(1, Page);
        public int RecordsPerPage { get; init; } = Math.Clamp(RecordsPerPage, 1, MaxRecordsPerPage);
    }
}
