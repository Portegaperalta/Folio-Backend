namespace Folio.Core.Domain.Exceptions
{
    public class EmptyUserPasswordHashException : Exception
    {
        public EmptyUserPasswordHashException() 
            : base($"User password hash cannot be empty") { }
    }
}
