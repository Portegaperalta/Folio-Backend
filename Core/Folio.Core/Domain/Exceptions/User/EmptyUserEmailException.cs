namespace Folio.Core.Domain.Exceptions.User
{
    public class EmptyUserEmailException : Exception
    {
        public EmptyUserEmailException() : base($"User email cannot be empty") { }
    }
}
