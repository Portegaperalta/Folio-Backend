namespace Folio.Core.Application.Exceptions
{
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException(string message = "Acess denied") 
            : base(message) { }
    }
}
