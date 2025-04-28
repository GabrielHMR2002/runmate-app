namespace RunMate.UserService.RunMate.Application.DTOs.EventDTOs
{
    public class EventCommentDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfilePicture { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
