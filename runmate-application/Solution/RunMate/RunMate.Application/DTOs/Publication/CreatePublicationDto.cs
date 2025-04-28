using RunMate.UserService.RunMate.Domain.Enums;

namespace RunMate.UserService.RunMate.Application.DTOs.Publication
{
    public record CreatePublicationDto(
        string Title,
        string Content,
        string? ImageUrl,
        PublicationType Type,
        PublicationVisibility Visibility
    );
}
