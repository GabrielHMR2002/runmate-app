using RunMate.UserService.RunMate.Domain.Entities;
using RunMate.UserService.RunMate.Domain.Enums;

namespace RunMate.UserService.RunMate.Application.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<EventEntity>> GetAllEventsAsync();
        Task<IEnumerable<EventEntity>> GetEventsByStatusAsync(EventStatus status);
        Task<IEnumerable<EventEntity>> GetEventsByOrganizerAsync(Guid organizerId);
        Task<IEnumerable<EventEntity>> GetEventsByParticipantAsync(Guid participantId);
        Task<IEnumerable<EventEntity>> GetUpcomingEventsAsync();
        Task<EventEntity> GetEventByIdAsync(Guid id);
        Task<EventEntity> CreateEventAsync(EventEntity eventEntity);
        Task<EventEntity> UpdateEventAsync(EventEntity eventEntity);
        Task DeleteEventAsync(Guid id);
        Task<bool> EventExistsAsync(Guid id);

        // Participantes
        Task<IEnumerable<EventParticipantEntity>> GetEventParticipantsAsync(Guid eventId);
        Task<EventParticipantEntity> GetEventParticipantAsync(Guid eventId, Guid userId);
        Task<EventParticipantEntity> AddParticipantAsync(EventParticipantEntity participant);
        Task<EventParticipantEntity> UpdateParticipantStatusAsync(Guid eventId, Guid userId, ParticipantStatus status);
        Task<bool> RemoveParticipantAsync(Guid eventId, Guid userId);

        // Checkpoints
        Task<IEnumerable<EventCheckpointEntity>> GetEventCheckpointsAsync(Guid eventId);
        Task<EventCheckpointEntity> AddCheckpointAsync(EventCheckpointEntity checkpoint);
        Task<bool> UpdateCheckpointAsync(EventCheckpointEntity checkpoint);
        Task<bool> DeleteCheckpointAsync(Guid checkpointId);

        // Comentários
        Task<IEnumerable<EventCommentEntity>> GetEventCommentsAsync(Guid eventId);
        Task<EventCommentEntity> AddCommentAsync(EventCommentEntity comment);
        Task<bool> UpdateCommentAsync(EventCommentEntity comment);
        Task<bool> DeleteCommentAsync(Guid commentId);
    }
}
