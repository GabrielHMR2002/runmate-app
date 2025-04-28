using Microsoft.EntityFrameworkCore;
using RunMate.Authentication.RunMate.Infrastructure.Persistence.Context;
using RunMate.UserService.RunMate.Application.Interfaces;
using RunMate.UserService.RunMate.Domain.Entities;
using RunMate.UserService.RunMate.Domain.Enums;

namespace RunMate.UserService.RunMate.Infrastructure.Persistence.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly RunMateContext _context;

        public EventRepository(RunMateContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventEntity>> GetAllEventsAsync()
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventEntity>> GetEventsByStatusAsync(EventStatus status)
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                .Where(e => e.Status == status)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventEntity>> GetEventsByOrganizerAsync(Guid organizerId)
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                .Where(e => e.OrganizerId == organizerId)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventEntity>> GetEventsByParticipantAsync(Guid participantId)
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                .Where(e => e.Participants.Any(p => p.UserId == participantId))
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventEntity>> GetUpcomingEventsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                .Where(e => e.StartDate > now && e.Status == EventStatus.Upcoming)
                .OrderBy(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<EventEntity> GetEventByIdAsync(Guid id)
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                    .ThenInclude(p => p.User)
                .Include(e => e.Checkpoints)
                .Include(e => e.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<EventEntity> CreateEventAsync(EventEntity eventEntity)
        {
            await _context.Events.AddAsync(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity;
        }

        public async Task<EventEntity> UpdateEventAsync(EventEntity eventEntity)
        {
            eventEntity.UpdatedAt = DateTime.UtcNow;
            _context.Events.Update(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity;
        }

        public async Task DeleteEventAsync(Guid id)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity != null)
            {
                _context.Events.Remove(eventEntity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> EventExistsAsync(Guid id)
        {
            return await _context.Events.AnyAsync(e => e.Id == id);
        }

        // Participantes
        public async Task<IEnumerable<EventParticipantEntity>> GetEventParticipantsAsync(Guid eventId)
        {
            return await _context.EventParticipants
                .Include(p => p.User)
                .Where(p => p.EventId == eventId)
                .ToListAsync();
        }

        public async Task<EventParticipantEntity> GetEventParticipantAsync(Guid eventId, Guid userId)
        {
            return await _context.EventParticipants
                .FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId);
        }

        public async Task<EventParticipantEntity> AddParticipantAsync(EventParticipantEntity participant)
        {
            await _context.EventParticipants.AddAsync(participant);
            await _context.SaveChangesAsync();
            return participant;
        }

        public async Task<EventParticipantEntity> UpdateParticipantStatusAsync(Guid eventId, Guid userId, ParticipantStatus status)
        {
            var participant = await _context.EventParticipants
                .FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId);

            if (participant != null)
            {
                participant.Status = status;
                await _context.SaveChangesAsync();
            }

            return participant;
        }

        public async Task<bool> RemoveParticipantAsync(Guid eventId, Guid userId)
        {
            var participant = await _context.EventParticipants
                .FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId);

            if (participant != null)
            {
                _context.EventParticipants.Remove(participant);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        // Checkpoints
        public async Task<IEnumerable<EventCheckpointEntity>> GetEventCheckpointsAsync(Guid eventId)
        {
            return await _context.EventCheckpoints
                .Where(c => c.EventId == eventId)
                .OrderBy(c => c.Order)
                .ToListAsync();
        }

        public async Task<EventCheckpointEntity> AddCheckpointAsync(EventCheckpointEntity checkpoint)
        {
            await _context.EventCheckpoints.AddAsync(checkpoint);
            await _context.SaveChangesAsync();
            return checkpoint;
        }

        public async Task<bool> UpdateCheckpointAsync(EventCheckpointEntity checkpoint)
        {
            _context.EventCheckpoints.Update(checkpoint);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteCheckpointAsync(Guid checkpointId)
        {
            var checkpoint = await _context.EventCheckpoints.FindAsync(checkpointId);
            if (checkpoint != null)
            {
                _context.EventCheckpoints.Remove(checkpoint);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }

        // Comentários
        public async Task<IEnumerable<EventCommentEntity>> GetEventCommentsAsync(Guid eventId)
        {
            return await _context.EventComments
                .Include(c => c.User)
                .Where(c => c.EventId == eventId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<EventCommentEntity> AddCommentAsync(EventCommentEntity comment)
        {
            await _context.EventComments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> UpdateCommentAsync(EventCommentEntity comment)
        {
            comment.UpdatedAt = DateTime.UtcNow;
            _context.EventComments.Update(comment);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId)
        {
            var comment = await _context.EventComments.FindAsync(commentId);
            if (comment != null)
            {
                _context.EventComments.Remove(comment);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }
    }
}
