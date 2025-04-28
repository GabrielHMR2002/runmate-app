using RunMate.Domain.Entities;
using RunMate.UserService.RunMate.Domain.Enums;

namespace RunMate.UserService.RunMate.Domain.Entities
{
    public class EventParticipantEntity
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public ParticipantStatus Status { get; set; } = ParticipantStatus.Registered;
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public TimeSpan? CompletionTime { get; set; }
        public int? Position { get; set; }

        // Propriedades de navegação
        public virtual EventEntity Event { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
