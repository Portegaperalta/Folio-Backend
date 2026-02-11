namespace Folio.Core.Domain.Exceptions.Bookmark
{
    public class EmptyBookmarkUrlException : Exception
    {
        public EmptyBookmarkUrlException() : base("Bookmark Url cannot be empty") { }
    }
}
