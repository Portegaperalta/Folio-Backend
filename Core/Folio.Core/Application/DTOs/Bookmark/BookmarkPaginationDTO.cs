namespace Folio.Core.Application.DTOs.Bookmark
{
    public record BookmarkPaginationDTO(int Page = 1, int RecordsPerPage = 30)
    {
        private const int MaxRecordsPerPage = 50;

        public int Page { get; init; } = Math.Max(1, Page);
        public int RecordsPerPage { get; init; } = Math.Clamp(RecordsPerPage, 1, MaxRecordsPerPage);
    }
}
