using System.ComponentModel.DataAnnotations;

namespace RunMate.UserService.RunMate.Application.DTOs.EventDTOs
{
    public class CreateCheckpointDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public double DistanceFromStart { get; set; }

        [Required]
        public int Order { get; set; }
    }
}
