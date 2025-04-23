namespace RunMate.EngagementService.RunMate.EngagementService.Application.DTOs.User
{
    public record UserDto(
        Guid Id,
        string Username,
        string Email,
        string? FullName,
        DateTime BirthDate,
        string Role,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        DateTime? LastLogin,
        bool IsActive
    );
}
