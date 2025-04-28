using RunMate.Domain.Entities;

namespace RunMate.UserService.RunMate.Domain.Entities
{
    public class PublicationLikeEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid PublicationId { get; set; }
        public virtual PublicationEntity Publication { get; set; }
        public Guid UserId { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
