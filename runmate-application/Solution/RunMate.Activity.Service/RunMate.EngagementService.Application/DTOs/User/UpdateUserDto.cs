namespace RunMate.EngagementService.RunMate.EngagementService.Application.DTOs.User
{
    public record UpdateUserDto(
        string? Email,
        string? FullName,
        DateTime? BirthDate,
        string? Role
    );
}
