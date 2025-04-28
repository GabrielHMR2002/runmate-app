namespace RunMate.UserService.RunMate.Application.DTOs.Publication
{
    public record UserSummaryDto(
        Guid Id,
        string Username,
        string FullName,
        string? ProfilePictureUrl
    );
}
