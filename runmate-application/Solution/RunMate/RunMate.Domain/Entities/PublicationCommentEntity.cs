using RunMate.Domain.Entities;

namespace RunMate.UserService.RunMate.Domain.Entities
{
    public class PublicationCommentEntity
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid PublicationId { get; set; }
        public virtual PublicationEntity Publication { get; set; }
        public Guid AuthorId { get; set; }
        public virtual UserEntity Author { get; set; }
    }
}
