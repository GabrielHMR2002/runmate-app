using RunMate.RunMate.Application.DTOs.UserDTOs;
using RunMate.UserService.RunMate.Domain.Enums;

namespace RunMate.UserService.RunMate.Application.DTOs.Publication
{
    public record PublicationDto(
    Guid Id,
    string Title,
    string Content,
    string? ImageUrl,
    PublicationType Type,
    DateTime CreatedAt,
    UserSummaryDto Author,
    int CommentsCount,
    int LikesCount,
    bool IsLikedByCurrentUser
);
}
