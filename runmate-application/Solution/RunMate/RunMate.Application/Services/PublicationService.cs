using AutoMapper;
using RunMate.UserService.RunMate.Application.DTOs.Publication;
using RunMate.UserService.RunMate.Application.Interfaces;
using RunMate.UserService.RunMate.Domain.Entities;
using RunMate.UserService.RunMate.Domain.Enums;
using RunMate.UserService.RunMate.Infrastructure.Persistence.Interfaces;

namespace RunMate.UserService.RunMate.Application.Services
{
    public class PublicationService : IPublicationService
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly IMapper _mapper;

        public PublicationService(IPublicationRepository publicationRepository, IMapper mapper)
        {
            _publicationRepository = publicationRepository;
            _mapper = mapper;
        }

        public async Task<PublicationDto?> GetByIdAsync(Guid id, Guid currentUserId)
        {
            var publication = await _publicationRepository.GetByIdAsync(id);
            if (publication == null)
                return null;

            var isLiked = await _publicationRepository.IsLikedByUserAsync(id, currentUserId);
            var likesCount = await _publicationRepository.GetLikesCountAsync(id);
            var commentsCount = await _publicationRepository.GetCommentsCountAsync(id);

            var authorDto = _mapper.Map<UserSummaryDto>(publication.Author);

            return new PublicationDto(
                publication.Id,
                publication.Title,
                publication.Content,
                publication.ImageUrl,
                publication.Type,
                publication.CreatedAt,
                authorDto,
                commentsCount,
                likesCount,
                isLiked
            );
        }

        public async Task<IEnumerable<PublicationDto>> GetFeedAsync(FeedParamsDto parameters, Guid currentUserId)
        {
            IEnumerable<PublicationEntity> publications;

            switch (parameters.FeedType)
            {
                case FeedType.Personal:
                    publications = await _publicationRepository.GetPersonalFeedAsync(
                        currentUserId, parameters.PageNumber, parameters.PageSize);
                    break;
                case FeedType.Friends:
                    publications = await _publicationRepository.GetFriendsFeedAsync(
                        currentUserId, parameters.PageNumber, parameters.PageSize);
                    break;
                default: // General
                    publications = await _publicationRepository.GetGeneralFeedAsync(
                        parameters.PageNumber, parameters.PageSize);
                    break;
            }

            var result = new List<PublicationDto>();

            foreach (var pub in publications)
            {
                var isLiked = await _publicationRepository.IsLikedByUserAsync(pub.Id, currentUserId);
                var likesCount = await _publicationRepository.GetLikesCountAsync(pub.Id);
                var commentsCount = await _publicationRepository.GetCommentsCountAsync(pub.Id);
                var authorDto = _mapper.Map<UserSummaryDto>(pub.Author);

                result.Add(new PublicationDto(
                    pub.Id,
                    pub.Title,
                    pub.Content,
                    pub.ImageUrl,
                    pub.Type,
                    pub.CreatedAt,
                    authorDto,
                    commentsCount,
                    likesCount,
                    isLiked
                ));
            }

            return result;
        }

        public async Task<PublicationDto> CreateAsync(CreatePublicationDto dto, Guid currentUserId)
        {
            var publication = new PublicationEntity
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                Type = dto.Type,
                Visibility = dto.Visibility,
                AuthorId = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _publicationRepository.CreateAsync(publication);

            // Recarregar a publicação com o autor
            var created = await _publicationRepository.GetByIdAsync(publication.Id);
            var authorDto = _mapper.Map<UserSummaryDto>(created.Author);

            return new PublicationDto(
                created.Id,
                created.Title,
                created.Content,
                created.ImageUrl,
                created.Type,
                created.CreatedAt,
                authorDto,
                0, // Comentários
                0, // Curtidas
                false // Não curtido pelo criador
            );
        }

        public async Task<PublicationDto> UpdateAsync(Guid id, UpdatePublicationDto dto, Guid currentUserId)
        {
            var publication = await _publicationRepository.GetByIdAsync(id);
            if (publication == null)
                throw new KeyNotFoundException($"Publicação com ID {id} não encontrada");

            if (publication.AuthorId != currentUserId)
                throw new UnauthorizedAccessException("Você não tem permissão para atualizar esta publicação");

            publication.Title = dto.Title;
            publication.Content = dto.Content;
            publication.ImageUrl = dto.ImageUrl;
            publication.Visibility = dto.Visibility;
            publication.UpdatedAt = DateTime.UtcNow;

            await _publicationRepository.UpdateAsync(publication);

            var isLiked = await _publicationRepository.IsLikedByUserAsync(id, currentUserId);
            var likesCount = await _publicationRepository.GetLikesCountAsync(id);
            var commentsCount = await _publicationRepository.GetCommentsCountAsync(id);
            var authorDto = _mapper.Map<UserSummaryDto>(publication.Author);

            return new PublicationDto(
                publication.Id,
                publication.Title,
                publication.Content,
                publication.ImageUrl,
                publication.Type,
                publication.CreatedAt,
                authorDto,
                commentsCount,
                likesCount,
                isLiked
            );
        }

        public async Task DeleteAsync(Guid id, Guid currentUserId)
        {
            var publication = await _publicationRepository.GetByIdAsync(id);
            if (publication == null)
                throw new KeyNotFoundException($"Publicação com ID {id} não encontrada");

            if (publication.AuthorId != currentUserId)
                throw new UnauthorizedAccessException("Você não tem permissão para excluir esta publicação");

            await _publicationRepository.DeleteAsync(id);
        }

        public async Task<LikeResponseDto> LikeAsync(Guid id, Guid currentUserId)
        {
            var publication = await _publicationRepository.GetByIdAsync(id);
            if (publication == null)
                throw new KeyNotFoundException($"Publicação com ID {id} não encontrada");

            var success = await _publicationRepository.LikePublicationAsync(id, currentUserId);
            var newCount = await _publicationRepository.GetLikesCountAsync(id);

            return new LikeResponseDto(success, newCount);
        }

        public async Task<LikeResponseDto> UnlikeAsync(Guid id, Guid currentUserId)
        {
            var publication = await _publicationRepository.GetByIdAsync(id);
            if (publication == null)
                throw new KeyNotFoundException($"Publicação com ID {id} não encontrada");

            var success = await _publicationRepository.UnlikePublicationAsync(id, currentUserId);
            var newCount = await _publicationRepository.GetLikesCountAsync(id);

            return new LikeResponseDto(success, newCount);
        }

        public async Task<CommentDto> AddCommentAsync(Guid publicationId, PublicationCommentDto dto, Guid currentUserId)
        {
            var publication = await _publicationRepository.GetByIdAsync(publicationId);
            if (publication == null)
                throw new KeyNotFoundException($"Publicação com ID {publicationId} não encontrada");

            var comment = new PublicationCommentEntity
            {
                Id = Guid.NewGuid(),
                Content = dto.Content,
                PublicationId = publicationId,
                AuthorId = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _publicationRepository.AddCommentAsync(comment);

            // Recarregar o comentário com o autor
            var comments = await _publicationRepository.GetCommentsAsync(publicationId, 1, 1);
            var createdComment = comments.First(c => c.Id == comment.Id);
            var authorDto = _mapper.Map<UserSummaryDto>(createdComment.Author);

            return new CommentDto(
                createdComment.Id,
                createdComment.Content,
                createdComment.CreatedAt,
                authorDto
            );
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsAsync(Guid publicationId, CommentListParamsDto parameters)
        {
            var publication = await _publicationRepository.GetByIdAsync(publicationId);
            if (publication == null)
                throw new KeyNotFoundException($"Publicação com ID {publicationId} não encontrada");

            var comments = await _publicationRepository.GetCommentsAsync(
                publicationId, parameters.PageNumber, parameters.PageSize);

            var result = new List<CommentDto>();

            foreach (var comment in comments)
            {
                var authorDto = _mapper.Map<UserSummaryDto>(comment.Author);

                result.Add(new CommentDto(
                    comment.Id,
                    comment.Content,
                    comment.CreatedAt,
                    authorDto
                ));
            }

            return result;
        }
    }
}
