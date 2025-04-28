using RunMate.UserService.RunMate.Application.DTOs.EventDTOs;
using RunMate.UserService.RunMate.Application.Interfaces;
using RunMate.UserService.RunMate.Domain.Entities;
using RunMate.UserService.RunMate.Domain.Enums;

namespace RunMate.UserService.RunMate.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return events.Select(MapEventEntityToDto);
        }

        public async Task<IEnumerable<EventDto>> GetEventsByStatusAsync(EventStatus status)
        {
            var events = await _eventRepository.GetEventsByStatusAsync(status);
            return events.Select(MapEventEntityToDto);
        }

        public async Task<IEnumerable<EventDto>> GetEventsByOrganizerAsync(Guid organizerId)
        {
            var events = await _eventRepository.GetEventsByOrganizerAsync(organizerId);
            return events.Select(MapEventEntityToDto);
        }

        public async Task<IEnumerable<EventDto>> GetEventsByParticipantAsync(Guid participantId)
        {
            var events = await _eventRepository.GetEventsByParticipantAsync(participantId);
            return events.Select(MapEventEntityToDto);
        }

        public async Task<IEnumerable<EventDto>> GetUpcomingEventsAsync()
        {
            var events = await _eventRepository.GetUpcomingEventsAsync();
            return events.Select(MapEventEntityToDto);
        }

        public async Task<EventDto> GetEventByIdAsync(Guid id)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(id);
            return eventEntity != null ? MapEventEntityToDto(eventEntity) : null;
        }

        public async Task<EventDto> CreateEventAsync(CreateEventDto createEventDto, Guid organizerId)
        {
            var eventEntity = new EventEntity
            {
                Id = Guid.NewGuid(),
                Name = createEventDto.Name,
                Description = createEventDto.Description,
                StartDate = createEventDto.StartDate,
                EndDate = createEventDto.EndDate,
                Location = createEventDto.Location,
                Distance = createEventDto.Distance,
                Type = createEventDto.Type,
                Status = EventStatus.Upcoming,
                MaxParticipants = createEventDto.MaxParticipants,
                OrganizerId = organizerId,
                ImageUrl = createEventDto.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            var createdEvent = await _eventRepository.CreateEventAsync(eventEntity);

            // Adiciona checkpoints se fornecidos
            if (createEventDto.Checkpoints != null && createEventDto.Checkpoints.Any())
            {
                foreach (var checkpointDto in createEventDto.Checkpoints)
                {
                    await AddCheckpointInternalAsync(createdEvent.Id, checkpointDto);
                }
            }

            return await GetEventByIdAsync(createdEvent.Id);
        }

        public async Task<EventDto> UpdateEventAsync(UpdateEventDto updateEventDto)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(updateEventDto.Id);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Event with ID {updateEventDto.Id} not found.");

            eventEntity.Name = updateEventDto.Name;
            eventEntity.Description = updateEventDto.Description;
            eventEntity.StartDate = updateEventDto.StartDate;
            eventEntity.EndDate = updateEventDto.EndDate;
            eventEntity.Location = updateEventDto.Location;
            eventEntity.Distance = updateEventDto.Distance;
            eventEntity.MaxParticipants = updateEventDto.MaxParticipants;
            eventEntity.ImageUrl = updateEventDto.ImageUrl;
            eventEntity.Status = updateEventDto.Status;
            eventEntity.UpdatedAt = DateTime.UtcNow;

            await _eventRepository.UpdateEventAsync(eventEntity);

            return await GetEventByIdAsync(eventEntity.Id);
        }

        public async Task DeleteEventAsync(Guid id, Guid currentUserId)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(id);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Event with ID {id} not found.");

            if (eventEntity.OrganizerId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to delete this event.");

            await _eventRepository.DeleteEventAsync(id);
        }

        // Participantes
        public async Task<IEnumerable<EventParticipantDto>> GetEventParticipantsAsync(Guid eventId)
        {
            var participants = await _eventRepository.GetEventParticipantsAsync(eventId);
            return participants.Select(p => new EventParticipantDto
            {
                Id = p.Id,
                EventId = p.EventId,
                UserId = p.UserId,
                UserName = p.User.Username,
                //UserProfilePicture = p.User.ProfilePicture,
                Status = p.Status,
                RegistrationDate = p.RegistrationDate,
                CompletionTime = p.CompletionTime,
                Position = p.Position
            });
        }

        public async Task<EventParticipantDto> RegisterParticipantAsync(Guid eventId, Guid userId)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(eventId);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Event with ID {eventId} not found.");

            if (eventEntity.Status != EventStatus.Upcoming)
                throw new InvalidOperationException("Registration is only available for upcoming events.");

            var existingParticipant = await _eventRepository.GetEventParticipantAsync(eventId, userId);
            if (existingParticipant != null)
                throw new InvalidOperationException("User is already registered for this event.");

            if (eventEntity.MaxParticipants > 0 && eventEntity.Participants.Count >= eventEntity.MaxParticipants)
                throw new InvalidOperationException("Event has reached maximum number of participants.");

            var participant = new EventParticipantEntity
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                UserId = userId,
                Status = ParticipantStatus.Registered,
                RegistrationDate = DateTime.UtcNow
            };

            var createdParticipant = await _eventRepository.AddParticipantAsync(participant);

            return new EventParticipantDto
            {
                Id = createdParticipant.Id,
                EventId = createdParticipant.EventId,
                UserId = createdParticipant.UserId,
                Status = createdParticipant.Status,
                RegistrationDate = createdParticipant.RegistrationDate
            };
        }

        public async Task<EventParticipantDto> UpdateParticipantStatusAsync(Guid eventId, Guid userId, ParticipantStatus status)
        {
            var participant = await _eventRepository.GetEventParticipantAsync(eventId, userId);
            if (participant == null)
                throw new KeyNotFoundException($"Participant not found for event ID {eventId} and user ID {userId}.");

            var updatedParticipant = await _eventRepository.UpdateParticipantStatusAsync(eventId, userId, status);

            return new EventParticipantDto
            {
                Id = updatedParticipant.Id,
                EventId = updatedParticipant.EventId,
                UserId = updatedParticipant.UserId,
                Status = updatedParticipant.Status,
                RegistrationDate = updatedParticipant.RegistrationDate,
                CompletionTime = updatedParticipant.CompletionTime,
                Position = updatedParticipant.Position
            };
        }

        public async Task<bool> RemoveParticipantAsync(Guid eventId, Guid userId, Guid currentUserId)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(eventId);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Event with ID {eventId} not found.");

            // Apenas o organizador ou o próprio participante pode remover
            if (eventEntity.OrganizerId != currentUserId && userId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to remove this participant.");

            return await _eventRepository.RemoveParticipantAsync(eventId, userId);
        }

        // Checkpoints
        public async Task<IEnumerable<EventCheckpointDto>> GetEventCheckpointsAsync(Guid eventId)
        {
            var checkpoints = await _eventRepository.GetEventCheckpointsAsync(eventId);
            return checkpoints.Select(c => new EventCheckpointDto
            {
                Id = c.Id,
                EventId = c.EventId,
                Name = c.Name,
                Latitude = c.Latitude,
                Longitude = c.Longitude,
                DistanceFromStart = c.DistanceFromStart,
                Order = c.Order
            });
        }

        public async Task<EventCheckpointDto> AddCheckpointAsync(Guid eventId, CreateCheckpointDto checkpointDto, Guid currentUserId)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(eventId);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Event with ID {eventId} not found.");

            if (eventEntity.OrganizerId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to add checkpoints to this event.");

            return await AddCheckpointInternalAsync(eventId, checkpointDto);
        }

        private async Task<EventCheckpointDto> AddCheckpointInternalAsync(Guid eventId, CreateCheckpointDto checkpointDto)
        {
            var checkpoint = new EventCheckpointEntity
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                Name = checkpointDto.Name,
                Latitude = checkpointDto.Latitude,
                Longitude = checkpointDto.Longitude,
                DistanceFromStart = checkpointDto.DistanceFromStart,
                Order = checkpointDto.Order
            };

            var createdCheckpoint = await _eventRepository.AddCheckpointAsync(checkpoint);

            return new EventCheckpointDto
            {
                Id = createdCheckpoint.Id,
                EventId = createdCheckpoint.EventId,
                Name = createdCheckpoint.Name,
                Latitude = createdCheckpoint.Latitude,
                Longitude = createdCheckpoint.Longitude,
                DistanceFromStart = createdCheckpoint.DistanceFromStart,
                Order = createdCheckpoint.Order
            };
        }

        public async Task<bool> UpdateCheckpointAsync(EventCheckpointDto checkpointDto, Guid currentUserId)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(checkpointDto.EventId);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Event with ID {checkpointDto.EventId} not found.");

            if (eventEntity.OrganizerId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to update checkpoints for this event.");

            var checkpoint = new EventCheckpointEntity
            {
                Id = checkpointDto.Id,
                EventId = checkpointDto.EventId,
                Name = checkpointDto.Name,
                Latitude = checkpointDto.Latitude,
                Longitude = checkpointDto.Longitude,
                DistanceFromStart = checkpointDto.DistanceFromStart,
                Order = checkpointDto.Order
            };

            return await _eventRepository.UpdateCheckpointAsync(checkpoint);
        }

        public async Task<bool> DeleteCheckpointAsync(Guid checkpointId, Guid currentUserId)
        {
            var checkpoints = await _eventRepository.GetEventCheckpointsAsync(Guid.Empty);
            var checkpoint = checkpoints.FirstOrDefault(c => c.Id == checkpointId);

            if (checkpoint == null)
                throw new KeyNotFoundException($"Checkpoint with ID {checkpointId} not found.");

            var eventEntity = await _eventRepository.GetEventByIdAsync(checkpoint.EventId);
            if (eventEntity.OrganizerId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to delete checkpoints from this event.");

            return await _eventRepository.DeleteCheckpointAsync(checkpointId);
        }

        // Comentários
        public async Task<IEnumerable<EventCommentDto>> GetEventCommentsAsync(Guid eventId)
        {
            var comments = await _eventRepository.GetEventCommentsAsync(eventId);
            return comments.Select(c => new EventCommentDto
            {
                Id = c.Id,
                EventId = c.EventId,
                UserId = c.UserId,
                UserName = c.User.Username,
                //UserProfilePicture = c.User.ProfilePicture,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<EventCommentDto> AddCommentAsync(CreateCommentDto commentDto, Guid currentUserId)
        {
            var comment = new EventCommentEntity
            {
                Id = Guid.NewGuid(),
                EventId = commentDto.EventId,
                UserId = currentUserId,
                Content = commentDto.Content,
                CreatedAt = DateTime.UtcNow
            };

            var createdComment = await _eventRepository.AddCommentAsync(comment);

            // Precisamos buscar o usuário para retornar informações completas
            var comments = await _eventRepository.GetEventCommentsAsync(commentDto.EventId);
            var fullComment = comments.FirstOrDefault(c => c.Id == createdComment.Id);

            return new EventCommentDto
            {
                Id = fullComment.Id,
                EventId = fullComment.EventId,
                UserId = fullComment.UserId,
                UserName = fullComment.User.Username,
                //UserProfilePicture = fullComment.User.ProfilePicture,
                Content = fullComment.Content,
                CreatedAt = fullComment.CreatedAt,
                UpdatedAt = fullComment.UpdatedAt
            };
        }

        public async Task<EventCommentDto> UpdateCommentAsync(UpdateCommentDto commentDto, Guid currentUserId)
        {
            var comments = await _eventRepository.GetEventCommentsAsync(Guid.Empty);
            var existingComment = comments.FirstOrDefault(c => c.Id == commentDto.Id);

            if (existingComment == null)
                throw new KeyNotFoundException($"Comment with ID {commentDto.Id} not found.");

            if (existingComment.UserId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to update this comment.");

            existingComment.Content = commentDto.Content;
            existingComment.UpdatedAt = DateTime.UtcNow;

            await _eventRepository.UpdateCommentAsync(existingComment);

            return new EventCommentDto
            {
                Id = existingComment.Id,
                EventId = existingComment.EventId,
                UserId = existingComment.UserId,
                UserName = existingComment.User.Username,
                //UserProfilePicture = existingComment.User.ProfilePicture,
                Content = existingComment.Content,
                CreatedAt = existingComment.CreatedAt,
                UpdatedAt = existingComment.UpdatedAt
            };
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId, Guid currentUserId)
        {
            var comments = await _eventRepository.GetEventCommentsAsync(Guid.Empty);
            var comment = comments.FirstOrDefault(c => c.Id == commentId);

            if (comment == null)
                throw new KeyNotFoundException($"Comment with ID {commentId} not found.");

            // Verificar se o usuário é o autor do comentário ou organizador do evento
            var eventEntity = await _eventRepository.GetEventByIdAsync(comment.EventId);
            if (comment.UserId != currentUserId && eventEntity.OrganizerId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to delete this comment.");

            return await _eventRepository.DeleteCommentAsync(commentId);
        }

        // Helper Methods
        private EventDto MapEventEntityToDto(EventEntity eventEntity)
        {
            return new EventDto
            {
                Id = eventEntity.Id,
                Name = eventEntity.Name,
                Description = eventEntity.Description,
                StartDate = eventEntity.StartDate,
                EndDate = eventEntity.EndDate,
                Location = eventEntity.Location,
                Distance = eventEntity.Distance,
                Type = eventEntity.Type,
                Status = eventEntity.Status,
                MaxParticipants = eventEntity.MaxParticipants,
                CurrentParticipants = eventEntity.Participants?.Count ?? 0,
                OrganizerId = eventEntity.OrganizerId,
                OrganizerName = eventEntity.Organizer?.FullName ?? "Unknown",
                ImageUrl = eventEntity.ImageUrl,
                Checkpoints = eventEntity.Checkpoints?.Select(c => new EventCheckpointDto
                {
                    Id = c.Id,
                    EventId = c.EventId,
                    Name = c.Name,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude,
                    DistanceFromStart = c.DistanceFromStart,
                    Order = c.Order
                })?.ToList(),
                CreatedAt = eventEntity.CreatedAt
            };
        }
    }
}
