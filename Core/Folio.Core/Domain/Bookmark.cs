namespace Folio.Core.Domain
{
    internal class Bookmark
    {
        //Attributes
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Url { get; private set; }
        public bool IsMarkedFavorite { get; private set; }
        public DateTime CreationDate { get; private set; }
        public DateTime? LastVisitedTime { get; private set; }
        public int FolderId { get; private set; }

        //Constructor
        public Bookmark(string name,string url,int folderId)
        {
            if (string.IsNullOrWhiteSpace(name) is true)
            {
                throw new ArgumentException("Bookmark name cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(url) is true)
            {
                throw new ArgumentException("Bookmark url cannot be empty");
            }

            if (folderId <= 0)
            {
                throw new ArgumentException("Folder ID cannot be less or equal than zero");
            }

            this.Name = name;
            this.Url = url;
            this.IsMarkedFavorite = false;
            this.CreationDate = DateTime.UtcNow;
            this.LastVisitedTime = null;
            this.FolderId = folderId;
        }

        //Behavioural methods
        public void ChangeName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName) is true)
            {
                throw new ArgumentException("Bookmark name cannot be empty");
            }

            this.Name = newName;
        }

        public void ChangeUrl(string newUrl)
        {
            if (string.IsNullOrWhiteSpace(newUrl) is true)
            {
                throw new ArgumentException("Bookmark url cannot be empty");
            }

            this.Url = newUrl;
        }

        public void MarkFavorite() => this.IsMarkedFavorite = true;

        public void UnmarkFavorite() => this.IsMarkedFavorite = false;

        public void Visit() => this.LastVisitedTime = DateTime.UtcNow;
    }
}
