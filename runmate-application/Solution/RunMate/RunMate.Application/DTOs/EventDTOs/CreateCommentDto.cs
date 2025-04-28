using System.ComponentModel.DataAnnotations;

namespace RunMate.UserService.RunMate.Application.DTOs.EventDTOs
{
    public class CreateCommentDto
    {
        public Guid EventId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }
    }
}
