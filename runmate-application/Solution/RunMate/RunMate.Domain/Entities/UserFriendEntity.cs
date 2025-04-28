using RunMate.Domain.Entities;

namespace RunMate.UserService.RunMate.Domain.Entities
{
    public class UserFriendEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public virtual UserEntity User { get; set; }
        public Guid FriendId { get; set; }
        public virtual UserEntity Friend { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
