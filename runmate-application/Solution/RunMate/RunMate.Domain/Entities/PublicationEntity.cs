using RunMate.Domain.Entities;
using RunMate.UserService.RunMate.Domain.Enums;

namespace RunMate.UserService.RunMate.Domain.Entities
{
    public class PublicationEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? ImageUrl { get; set; }
        public PublicationType Type { get; set; }
        public PublicationVisibility Visibility { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid AuthorId { get; set; }
        public virtual UserEntity Author { get; set; }
        public virtual ICollection<PublicationCommentEntity> Comments { get; set; }
        public virtual ICollection<PublicationLikeEntity> Likes { get; set; }
    }
}
