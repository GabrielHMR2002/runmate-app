using RunMate.UserService.RunMate.Domain.Enums;

namespace RunMate.UserService.RunMate.Application.DTOs.EventDTOs
{
    public class EventParticipantDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfilePicture { get; set; }
        public ParticipantStatus Status { get; set; }
        public DateTime RegistrationDate { get; set; }
        public TimeSpan? CompletionTime { get; set; }
        public int? Position { get; set; }
    }
}
