namespace Folio.Core.Domain.Exceptions
{
    public class EmptyFolderNameException : Exception
    {
        public EmptyFolderNameException() : base("Folder name cannot be empty") { }
    }
}
