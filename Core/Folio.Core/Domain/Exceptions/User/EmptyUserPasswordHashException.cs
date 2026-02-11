namespace Folio.Core.Domain.Exceptions.User
{
    public class EmptyUserPasswordHashException : Exception
    {
        public EmptyUserPasswordHashException() 
            : base($"User password hash cannot be empty") { }
    }
}
