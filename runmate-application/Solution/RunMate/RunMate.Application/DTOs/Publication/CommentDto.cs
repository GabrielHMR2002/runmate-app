namespace RunMate.UserService.RunMate.Application.DTOs.Publication
{
    public record CommentDto(
        Guid Id,
        string Content,
        DateTime CreatedAt,
        UserSummaryDto Author
    );
}
