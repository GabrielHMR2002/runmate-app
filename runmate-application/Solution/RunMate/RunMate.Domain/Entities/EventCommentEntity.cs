using RunMate.Domain.Entities;

namespace RunMate.UserService.RunMate.Domain.Entities
{
    public class EventCommentEntity
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Propriedades de navegação
        public virtual EventEntity Event { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
