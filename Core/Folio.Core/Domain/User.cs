namespace Folio.Core.Domain
{
    public class User
    {
        //Attributes
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string? PhoneNumber { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime CreationDate { get; private set; }

        private readonly List<Folder> _folders = [];
        public IReadOnlyCollection<Folder> Folders => _folders.AsReadOnly();

        private readonly List<Bookmark> _bookmarks = [];
        public IReadOnlyCollection<Bookmark> Bookmarks => _bookmarks.AsReadOnly();

        //Constructor
        public User(string name,string email,string passwordHash,string? phoneNumber = null)
        {
            if (string.IsNullOrWhiteSpace(name) is true)
            {
                throw new ArgumentException("User name cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(email) is true)
            {
                throw new ArgumentException("User email cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(passwordHash) is true)
            {
                throw new ArgumentException("User password hash cannot be empty");
            }

            this.Name = name;
            this.Email = email;
            this.PasswordHash = passwordHash;
            this.PhoneNumber = phoneNumber;
            this.IsDeleted = false;
            this.CreationDate = DateTime.UtcNow;
        }

        //Behavioural methods
        public void ChangeName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName) is true)
            {
                throw new ArgumentException("User name cannot be empty");
            }

            this.Name = newName;
        }

        public void ChangeEmail(string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail) is true)
            {
                throw new ArgumentException("User email cannot be empty");
            }

            this.Email = newEmail;
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash) is true)
            {
                throw new ArgumentException("User password cannot be empty");
            }

            this.PasswordHash = newPasswordHash;
        }

        public void SetPhoneNumber(string? phoneNumber)
        {
            this.PhoneNumber = phoneNumber;
        }

        public void Delete() => this.IsDeleted = true;
    }
}
