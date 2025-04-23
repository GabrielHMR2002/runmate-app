namespace RunMate.Shared.Auth.Event.User
{
    public class UserPasswordChangedEvent
    {
        public Guid UserId { get; set; }
        public string PasswordHash { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
