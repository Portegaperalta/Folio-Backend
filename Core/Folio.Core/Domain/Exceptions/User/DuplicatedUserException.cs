namespace Folio.Core.Domain.Exceptions.User
{
    public class DuplicatedUsernameException : Exception
    {
        public DuplicatedUsernameException() : base("Username is already taken.") { }
    }
}
