using Folio.Core.Domain.Exceptions;

namespace Folio.Core.Domain.Entities
{
    public class Bookmark
    {
        //Attributes
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsMarkedFavorite { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastVisitedTime { get; set; }
        public Guid FolderId { get; set; }
        public Guid UserId { get; set; }
        public Folder? Folder { get; set; }

        //Constructor
        public Bookmark(string name, string url, Guid folderId, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(name) is true)
            {
                throw new EmptyBookmarkNameException();
            }

            if (string.IsNullOrWhiteSpace(url) is true)
            {
                throw new EmptyBookmarkUrlException();
            }

            this.Name = name;
            this.Url = url;
            this.IsMarkedFavorite = false;
            this.CreationDate = DateTime.UtcNow;
            this.LastVisitedTime = null;
            this.FolderId = folderId;
            this.UserId = userId;
        }

        //Behavioural methods
        public void ChangeName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName) is true)
            {
                throw new EmptyBookmarkNameException();
            }

            this.Name = newName;
        }

        public void ChangeUrl(string newUrl)
        {
            if (string.IsNullOrWhiteSpace(newUrl) is true)
            {
                throw new EmptyBookmarkUrlException();
            }

            this.Url = newUrl;
        }

        public void MarkFavorite() => this.IsMarkedFavorite = true;

        public void UnmarkFavorite() => this.IsMarkedFavorite = false;

        public void Visit() => this.LastVisitedTime = DateTime.UtcNow;
    }
}
