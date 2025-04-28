using RunMate.Domain.Entities;
using RunMate.UserService.RunMate.Domain.Enums;

namespace RunMate.UserService.RunMate.Domain.Entities
{
    public class EventEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public double Distance { get; set; } // em km
        public EventType Type { get; set; }
        public EventStatus Status { get; set; } = EventStatus.Upcoming;
        public int MaxParticipants { get; set; }
        public Guid OrganizerId { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Propriedades de navegação
        public virtual UserEntity Organizer { get; set; }
        public virtual ICollection<EventParticipantEntity> Participants { get; set; }
        public virtual ICollection<EventCheckpointEntity> Checkpoints { get; set; }
        public virtual ICollection<EventCommentEntity> Comments { get; set; }
    }
}
