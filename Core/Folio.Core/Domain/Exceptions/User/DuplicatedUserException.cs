namespace Folio.Core.Domain.Exceptions.User
{
    public class RegistrationFailedException : Exception
    {
        public RegistrationFailedException() : base("Registration failed, one or more fields are invalid") { }
    }
}
