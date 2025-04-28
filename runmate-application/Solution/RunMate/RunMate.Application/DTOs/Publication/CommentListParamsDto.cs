namespace RunMate.UserService.RunMate.Application.DTOs.Publication
{
    public record CommentListParamsDto(
        int PageNumber = 1,
        int PageSize = 10
    );
}
