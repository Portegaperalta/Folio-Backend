namespace Folio.Core.Domain.Exceptions.Bookmark
{
    public class EmptyBookmarkNameException : Exception
    {
        public EmptyBookmarkNameException() : base("Bookmark name cannot be empty") { }
    }
}
