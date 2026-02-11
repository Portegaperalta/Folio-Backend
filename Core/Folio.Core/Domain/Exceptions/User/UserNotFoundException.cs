namespace Folio.Core.Domain.Exceptions.User
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(Guid userId)
            : base($"User with id {userId} not found") { }
    }
}
