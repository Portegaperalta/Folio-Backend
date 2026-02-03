namespace Folio.Core.Domain.Exceptions
{
    public class BookmarkNotFoundException : Exception
    {
        public BookmarkNotFoundException(Guid bookmarkId)
            : base($"Bookmark with id {bookmarkId} not found") { }
    }
}
