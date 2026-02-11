namespace Folio.Core.Domain.Exceptions
{
    public class EmptyUserEmailException : Exception
    {
        public EmptyUserEmailException() : base($"User email cannot be empty") { }
    }
}
