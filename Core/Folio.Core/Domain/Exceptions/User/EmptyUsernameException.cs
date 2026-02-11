namespace Folio.Core.Domain.Exceptions.User
{
    public class EmptyUsernameException : Exception
    {
        public EmptyUsernameException() : base("User name cannot be empty") { }
    }
}
