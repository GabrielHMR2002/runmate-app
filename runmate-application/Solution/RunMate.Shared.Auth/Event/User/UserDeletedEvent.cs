namespace RunMate.Shared.Auth.Event.User
{
    public class UserDeletedEvent
    {
        public Guid UserId { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
