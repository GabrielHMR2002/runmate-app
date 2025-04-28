using RunMate.UserService.RunMate.Domain.Enums;

namespace RunMate.UserService.RunMate.Application.DTOs.Publication
{
    public record FeedParamsDto(
        FeedType FeedType,
        int PageNumber = 1,
        int PageSize = 10
    );
}
