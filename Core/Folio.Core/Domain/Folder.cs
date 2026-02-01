namespace Folio.Core.Domain
{
    public class Folder
    {
        //Attributes
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsMarkedFavorite { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastVisitedTime { get; set; }
        public Guid UserId { get; set; }

        private readonly List<Bookmark> _bookmarks  = [];
        public IReadOnlyCollection<Bookmark> Bookmarks => _bookmarks.AsReadOnly();

        //Constructor
        public Folder(string name,Guid userId)
        {
            if (string.IsNullOrWhiteSpace(name) is true)
            {
                throw new ArgumentException("Name cannot be empty");
            }

            this.Name = name;
            this.IsMarkedFavorite = false;
            this.CreationDate = DateTime.UtcNow;
            this.LastVisitedTime = null;
            this.UserId = userId;
        }

        //Behavioural methods
        public void ChangeName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName) is true)
            {
                throw new ArgumentException("Name cannot be empty");
            }

            this.Name = newName;
        }

        public void MarkFavorite() => this.IsMarkedFavorite = true;

        public void UnmarkFavorite() => this.IsMarkedFavorite = false;

        public void Visit() => this.LastVisitedTime = DateTime.UtcNow;

        public void AddBookmark(Bookmark newBookmark)
        {
            if (newBookmark is null)
            {
                ArgumentNullException.ThrowIfNull(newBookmark);
            }

            if (newBookmark.FolderId != this.Id)
            {
                throw new ArgumentException("Bookmark doesn't belong to this folder");
            }

            this._bookmarks.Add(newBookmark);
        }

        public void RemoveBookmark(Guid bookmarkId)
        {
            var bookmark = this._bookmarks.FirstOrDefault(b => b.Id == bookmarkId);
            
            if (bookmark is not null)
            {
                this._bookmarks.Remove(bookmark);
            }
        }
    }
}
