namespace FolioWebAPI.DTOs.Bookmark
{
    public class BookmarkUpdateDTO
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public bool? IsMarkedFavorite { get; set; }
    }
}
