namespace FolioWebAPI.DTOs.Bookmark
{
    public class BookmarkDTO
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Url { get; set; }
        public required bool IsMarkedFavorite { get; set; }
        public required DateTime CreationDate { get; set; }
        public required DateTime? LastVisitedTime { get; set; }
    }
}
