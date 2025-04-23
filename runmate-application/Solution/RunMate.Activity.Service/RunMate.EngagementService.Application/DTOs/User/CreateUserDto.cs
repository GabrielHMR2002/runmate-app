namespace RunMate.EngagementService.RunMate.EngagementService.Application.DTOs.User
{
    public record CreateUserDto(
        string Username,
        string Password,
        string Email,
        string? FullName,
        DateTime BirthDate,
        string? Role
    );
}
