using RunMate.UserService.RunMate.Application.DTOs.EventDTOs;
using RunMate.UserService.RunMate.Domain.Enums;

namespace RunMate.UserService.RunMate.Application.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllEventsAsync();
        Task<IEnumerable<EventDto>> GetEventsByStatusAsync(EventStatus status);
        Task<IEnumerable<EventDto>> GetEventsByOrganizerAsync(Guid organizerId);
        Task<IEnumerable<EventDto>> GetEventsByParticipantAsync(Guid participantId);
        Task<IEnumerable<EventDto>> GetUpcomingEventsAsync();
        Task<EventDto> GetEventByIdAsync(Guid id);
        Task<EventDto> CreateEventAsync(CreateEventDto createEventDto, Guid organizerId);
        Task<EventDto> UpdateEventAsync(UpdateEventDto updateEventDto);
        Task DeleteEventAsync(Guid id, Guid currentUserId);

        // Participantes
        Task<IEnumerable<EventParticipantDto>> GetEventParticipantsAsync(Guid eventId);
        Task<EventParticipantDto> RegisterParticipantAsync(Guid eventId, Guid userId);
        Task<EventParticipantDto> UpdateParticipantStatusAsync(Guid eventId, Guid userId, ParticipantStatus status);
        Task<bool> RemoveParticipantAsync(Guid eventId, Guid userId, Guid currentUserId);

        // Checkpoints
        Task<IEnumerable<EventCheckpointDto>> GetEventCheckpointsAsync(Guid eventId);
        Task<EventCheckpointDto> AddCheckpointAsync(Guid eventId, CreateCheckpointDto checkpointDto, Guid currentUserId);
        Task<bool> UpdateCheckpointAsync(EventCheckpointDto checkpointDto, Guid currentUserId);
        Task<bool> DeleteCheckpointAsync(Guid checkpointId, Guid currentUserId);

        // Comentários
        Task<IEnumerable<EventCommentDto>> GetEventCommentsAsync(Guid eventId);
        Task<EventCommentDto> AddCommentAsync(CreateCommentDto commentDto, Guid currentUserId);
        Task<EventCommentDto> UpdateCommentAsync(UpdateCommentDto commentDto, Guid currentUserId);
        Task<bool> DeleteCommentAsync(Guid commentId, Guid currentUserId);
    }
}
