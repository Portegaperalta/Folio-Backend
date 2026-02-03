namespace Folio.Core.Domain.Exceptions
{
    public class FolderNotFoundException : Exception
    {
        public FolderNotFoundException(Guid folderId)
            : base($"Folder with ID {folderId} was not found") { }
    }
}
