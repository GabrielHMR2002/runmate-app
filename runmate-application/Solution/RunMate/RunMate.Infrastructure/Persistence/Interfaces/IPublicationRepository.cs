using RunMate.UserService.RunMate.Domain.Entities;

namespace RunMate.UserService.RunMate.Infrastructure.Persistence.Interfaces
{
    public interface IPublicationRepository
    {
        Task<PublicationEntity?> GetByIdAsync(Guid id);
        Task<IEnumerable<PublicationEntity>> GetGeneralFeedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<PublicationEntity>> GetPersonalFeedAsync(Guid userId, int pageNumber, int pageSize);
        Task<IEnumerable<PublicationEntity>> GetFriendsFeedAsync(Guid userId, int pageNumber, int pageSize);
        Task<PublicationEntity> CreateAsync(PublicationEntity publication);
        Task<PublicationEntity> UpdateAsync(PublicationEntity publication);
        Task DeleteAsync(Guid id);
        Task<bool> LikePublicationAsync(Guid publicationId, Guid userId);
        Task<bool> UnlikePublicationAsync(Guid publicationId, Guid userId);
        Task<PublicationCommentEntity> AddCommentAsync(PublicationCommentEntity comment);
        Task<IEnumerable<PublicationCommentEntity>> GetCommentsAsync(Guid publicationId, int pageNumber, int pageSize);
        Task<bool> IsLikedByUserAsync(Guid publicationId, Guid userId);
        Task<int> GetLikesCountAsync(Guid publicationId);
        Task<int> GetCommentsCountAsync(Guid publicationId);
    }
}
