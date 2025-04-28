using RunMate.UserService.RunMate.Domain.Enums;

namespace RunMate.UserService.RunMate.Application.DTOs.Publication
{
    public record UpdatePublicationDto(
        string Title,
        string Content,
        string? ImageUrl,
        PublicationVisibility Visibility
    );
}
