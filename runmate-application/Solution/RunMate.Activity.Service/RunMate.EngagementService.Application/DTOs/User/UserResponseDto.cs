namespace RunMate.EngagementService.RunMate.EngagementService.Application.DTOs.User
{
    public record UserResponseDto(
        bool Success,
        string Message,
        UserDto? User
    );
}
