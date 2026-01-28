namespace FolioWebAPI.DTOs.Folder
{
    public class FolderDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required bool IsMarkedFavorite { get; set; }
        public required DateTime CreationDate { get; set; }
        public DateTime? LastVisitedTime { get; set; }
    }
}
