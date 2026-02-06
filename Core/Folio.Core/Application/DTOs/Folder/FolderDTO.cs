namespace Folio.Core.Application.DTOs.Folder
{
    public class FolderDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required bool IsMarkedFavorite { get; set; }
        public required DateTime CreationDate { get; set; }
        public DateTime? LastVisitedTime { get; set; }
    }
}
