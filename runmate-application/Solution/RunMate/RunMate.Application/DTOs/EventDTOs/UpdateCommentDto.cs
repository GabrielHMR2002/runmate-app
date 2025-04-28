using System.ComponentModel.DataAnnotations;

namespace RunMate.UserService.RunMate.Application.DTOs.EventDTOs
{
    public class UpdateCommentDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }
    }
}
