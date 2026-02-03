namespace Folio.Core.Domain.Exceptions
{
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException(string message = "Acess denied") 
            : base(message) { }
    }
}
