using Microsoft.EntityFrameworkCore;
using RunMate.Authentication.RunMate.Infrastructure.Persistence.Context;
using RunMate.UserService.RunMate.Domain.Entities;
using RunMate.UserService.RunMate.Domain.Enums;
using RunMate.UserService.RunMate.Infrastructure.Persistence.Interfaces;

namespace RunMate.UserService.RunMate.Infrastructure.Persistence.Repositories
{
    public class PublicationRepository : IPublicationRepository
    {
        private readonly RunMateContext _context;

        public PublicationRepository(RunMateContext context)
        {
            _context = context;
        }

        public async Task<PublicationEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Publications
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PublicationEntity>> GetGeneralFeedAsync(int pageNumber, int pageSize)
        {
            return await _context.Publications
                .Where(p => p.Visibility == PublicationVisibility.Public)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Author)
                .ToListAsync();
        }

        public async Task<IEnumerable<PublicationEntity>> GetPersonalFeedAsync(Guid userId, int pageNumber, int pageSize)
        {
            return await _context.Publications
                .Where(p => p.AuthorId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Author)
                .ToListAsync();
        }

        public async Task<IEnumerable<PublicationEntity>> GetFriendsFeedAsync(Guid userId, int pageNumber, int pageSize)
        {
            var friendIds = await _context.UserFriends
                .Where(uf => uf.UserId == userId)
                .Select(uf => uf.FriendId)
                .ToListAsync();

            // Otimização: inclui outras informações relevantes
            return await _context.Publications
                .Where(p => friendIds.Contains(p.AuthorId) &&
                           (p.Visibility == PublicationVisibility.Public ||
                            p.Visibility == PublicationVisibility.Friends))
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Author)
                .Include(p => p.Likes.Where(l => l.UserId == userId))
                .ToListAsync();
        }

        public async Task<PublicationEntity> CreateAsync(PublicationEntity publication)
        {
            await _context.Publications.AddAsync(publication);
            await _context.SaveChangesAsync();
            return publication;
        }

        public async Task<PublicationEntity> UpdateAsync(PublicationEntity publication)
        {
            publication.UpdatedAt = DateTime.UtcNow;
            _context.Publications.Update(publication);
            await _context.SaveChangesAsync();
            return publication;
        }

        public async Task DeleteAsync(Guid id)
        {
            var publication = await _context.Publications.FindAsync(id);
            if (publication != null)
            {
                _context.Publications.Remove(publication);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> LikePublicationAsync(Guid publicationId, Guid userId)
        {
            var existingLike = await _context.PublicationLikes
                .FirstOrDefaultAsync(l => l.PublicationId == publicationId && l.UserId == userId);

            if (existingLike != null)
                return false;

            var like = new PublicationLikeEntity
            {
                Id = Guid.NewGuid(),
                PublicationId = publicationId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.PublicationLikes.AddAsync(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnlikePublicationAsync(Guid publicationId, Guid userId)
        {
            var like = await _context.PublicationLikes
                .FirstOrDefaultAsync(l => l.PublicationId == publicationId && l.UserId == userId);

            if (like == null)
                return false;

            _context.PublicationLikes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PublicationCommentEntity> AddCommentAsync(PublicationCommentEntity comment)
        {
            await _context.PublicationComments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<IEnumerable<PublicationCommentEntity>> GetCommentsAsync(Guid publicationId, int pageNumber, int pageSize)
        {
            return await _context.PublicationComments
                .Where(c => c.PublicationId == publicationId)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(c => c.Author)
                .ToListAsync();
        }

        public async Task<bool> IsLikedByUserAsync(Guid publicationId, Guid userId)
        {
            return await _context.PublicationLikes
                .AnyAsync(l => l.PublicationId == publicationId && l.UserId == userId);
        }

        public async Task<int> GetLikesCountAsync(Guid publicationId)
        {
            return await _context.PublicationLikes
                .CountAsync(l => l.PublicationId == publicationId);
        }

        public async Task<int> GetCommentsCountAsync(Guid publicationId)
        {
            return await _context.PublicationComments
                .CountAsync(c => c.PublicationId == publicationId);
        }
    }
}
