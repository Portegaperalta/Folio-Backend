namespace Folio.Core.Domain.Exceptions
{
    public class EmptyBookmarkNameException : Exception
    {
        public EmptyBookmarkNameException() : base("Bookmark name cannot be empty") { }
    }
}
