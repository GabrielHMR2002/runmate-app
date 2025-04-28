using RunMate.UserService.RunMate.Application.DTOs.Publication;

namespace RunMate.UserService.RunMate.Application.Interfaces
{
    public interface IPublicationService
    {
        Task<PublicationDto?> GetByIdAsync(Guid id, Guid currentUserId);
        Task<IEnumerable<PublicationDto>> GetFeedAsync(FeedParamsDto parameters, Guid currentUserId);
        Task<PublicationDto> CreateAsync(CreatePublicationDto dto, Guid currentUserId);
        Task<PublicationDto> UpdateAsync(Guid id, UpdatePublicationDto dto, Guid currentUserId);
        Task DeleteAsync(Guid id, Guid currentUserId);
        Task<LikeResponseDto> LikeAsync(Guid id, Guid currentUserId);
        Task<LikeResponseDto> UnlikeAsync(Guid id, Guid currentUserId);
        Task<CommentDto> AddCommentAsync(Guid publicationId, PublicationCommentDto dto, Guid currentUserId);
        Task<IEnumerable<CommentDto>> GetCommentsAsync(Guid publicationId, CommentListParamsDto parameters);
    }
}
