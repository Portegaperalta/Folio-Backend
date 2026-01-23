namespace Folio.Core.Domain
{
    public class Folder
    {
        //Attributes
        public int Id { get; private set; }
        public string Name { get; private set; }
        public bool IsMarkedFavorite { get; private set; }
        public DateTime CreationDate { get; private set; }
        public DateTime? LastVisitedTime { get; private set; }
        public int UserId { get; private set; }

        //Constructor
        public Folder(string name,int userId)
        {
            if (string.IsNullOrWhiteSpace(name) is true)
            {
                throw new ArgumentException("Name cannot be empty");
            }

            if (userId <= 0)
            {
                throw new ArgumentException("User id cannot be less or equal than zero");
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
    }
}
