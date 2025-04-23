namespace RunMate.Shared.Auth.Event.User
{
    public class UserStatusChangedEvent
    {
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
