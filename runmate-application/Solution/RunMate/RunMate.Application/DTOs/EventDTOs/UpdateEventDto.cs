using RunMate.UserService.RunMate.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace RunMate.UserService.RunMate.Application.DTOs.EventDTOs
{
    public class UpdateEventDto
    {
        public Guid Id { get; set; }

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

        public EventStatus Status { get; set; }

        public int MaxParticipants { get; set; }

        public string ImageUrl { get; set; }
    }
}
