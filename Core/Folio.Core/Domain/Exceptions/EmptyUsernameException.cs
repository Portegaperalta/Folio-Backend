namespace Folio.Core.Domain.Exceptions
{
    public class EmptyUsernameException : Exception
    {
        public EmptyUsernameException() : base("User name cannot be empty") { }
    }
}
