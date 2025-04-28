using RunMate.UserService.RunMate.Domain.Enums;

namespace RunMate.UserService.RunMate.Application.DTOs.EventDTOs
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public double Distance { get; set; }
        public EventType Type { get; set; }
        public EventStatus Status { get; set; }
        public int MaxParticipants { get; set; }
        public int CurrentParticipants { get; set; }
        public Guid OrganizerId { get; set; }
        public string OrganizerName { get; set; }
        public string ImageUrl { get; set; }
        public List<EventCheckpointDto> Checkpoints { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
