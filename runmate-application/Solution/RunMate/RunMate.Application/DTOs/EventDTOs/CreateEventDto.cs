using RunMate.UserService.RunMate.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace RunMate.UserService.RunMate.Application.DTOs.EventDTOs
{
    public class CreateEventDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public double Distance { get; set; }

        [Required]
        public EventType Type { get; set; }

        [Required]
        public int MaxParticipants { get; set; }

        public string ImageUrl { get; set; }

        public List<CreateCheckpointDto> Checkpoints { get; set; }
    }
}
