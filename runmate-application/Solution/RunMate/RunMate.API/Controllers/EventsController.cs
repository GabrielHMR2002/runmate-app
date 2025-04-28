using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RunMate.UserService.RunMate.Application.DTOs.EventDTOs;
using RunMate.UserService.RunMate.Application.Interfaces;
using RunMate.UserService.RunMate.Domain.Enums;
using System.Security.Claims;

namespace RunMate.UserService.RunMate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetAllEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEventsByStatus(EventStatus status)
        {
            var events = await _eventService.GetEventsByStatusAsync(status);
            return Ok(events);
        }

        [HttpGet("organized")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetOrganizedEvents()
        {
            var userId = GetCurrentUserId();
            var events = await _eventService.GetEventsByOrganizerAsync(userId);
            return Ok(events);
        }

        [HttpGet("participating")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetParticipatingEvents()
        {
            var userId = GetCurrentUserId();
            var events = await _eventService.GetEventsByParticipantAsync(userId);
            return Ok(events);
        }

        [HttpGet("upcoming")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetUpcomingEvents()
        {
            var events = await _eventService.GetUpcomingEventsAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventDto>> GetEvent(Guid id)
        {
            var eventDto = await _eventService.GetEventByIdAsync(id);
            if (eventDto == null)
                return NotFound();

            return Ok(eventDto);
        }

        [HttpPost]
        public async Task<ActionResult<EventDto>> CreateEvent(CreateEventDto createEventDto)
        {
            var userId = GetCurrentUserId();
            var createdEvent = await _eventService.CreateEventAsync(createEventDto, userId);
            return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
        }

        [HttpPut]
        public async Task<ActionResult<EventDto>> UpdateEvent(UpdateEventDto updateEventDto)
        {
            try
            {
                var updatedEvent = await _eventService.UpdateEventAsync(updateEventDto);
                return Ok(updatedEvent);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent(Guid id)
        {
            var userId = GetCurrentUserId();
            try
            {
                await _eventService.DeleteEventAsync(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // Endpoints para participantes
        [HttpGet("{id}/participants")]
        public async Task<ActionResult<IEnumerable<EventParticipantDto>>> GetEventParticipants(Guid id)
        {
            var participants = await _eventService.GetEventParticipantsAsync(id);
            return Ok(participants);
        }

        [HttpPost("{id}/participants")]
        public async Task<ActionResult<EventParticipantDto>> RegisterParticipant(Guid id)
        {
            var userId = GetCurrentUserId();
            try
            {
                var participant = await _eventService.RegisterParticipantAsync(id, userId);
                return Ok(participant);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/participants/{userId}/status")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult<EventParticipantDto>> UpdateParticipantStatus(Guid id, Guid userId, [FromBody] ParticipantStatus status)
        {
            try
            {
                var participant = await _eventService.UpdateParticipantStatusAsync(id, userId, status);
                return Ok(participant);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}/participants/{userId}")]
        public async Task<ActionResult> RemoveParticipant(Guid id, Guid userId)
        {
            var currentUserId = GetCurrentUserId();
            try
            {
                var result = await _eventService.RemoveParticipantAsync(id, userId, currentUserId);
                return result ? NoContent() : NotFound();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // Endpoints para checkpoints
        [HttpGet("{id}/checkpoints")]
        public async Task<ActionResult<IEnumerable<EventCheckpointDto>>> GetEventCheckpoints(Guid id)
        {
            var checkpoints = await _eventService.GetEventCheckpointsAsync(id);
            return Ok(checkpoints);
        }

        [HttpPost("{id}/checkpoints")]
        public async Task<ActionResult<EventCheckpointDto>> AddCheckpoint(Guid id, [FromBody] CreateCheckpointDto checkpointDto)
        {
            var userId = GetCurrentUserId();
            try
            {
                var checkpoint = await _eventService.AddCheckpointAsync(id, checkpointDto, userId);
                return Ok(checkpoint);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("checkpoints")]
        public async Task<ActionResult> UpdateCheckpoint([FromBody] EventCheckpointDto checkpointDto)
        {
            var userId = GetCurrentUserId();
            try
            {
                var result = await _eventService.UpdateCheckpointAsync(checkpointDto, userId);
                return result ? NoContent() : NotFound();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("checkpoints/{id}")]
        public async Task<ActionResult> DeleteCheckpoint(Guid id)
        {
            var userId = GetCurrentUserId();
            try
            {
                var result = await _eventService.DeleteCheckpointAsync(id, userId);
                return result ? NoContent() : NotFound();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // Endpoints para comentários
        [HttpGet("{id}/comments")]
        public async Task<ActionResult<IEnumerable<EventCommentDto>>> GetEventComments(Guid id)
        {
            var comments = await _eventService.GetEventCommentsAsync(id);
            return Ok(comments);
        }

        [HttpPost("comments")]
        public async Task<ActionResult<EventCommentDto>> AddComment([FromBody] CreateCommentDto commentDto)
        {
            var userId = GetCurrentUserId();
            try
            {
                var comment = await _eventService.AddCommentAsync(commentDto, userId);
                return Ok(comment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("comments")]
        public async Task<ActionResult<EventCommentDto>> UpdateComment([FromBody] UpdateCommentDto commentDto)
        {
            var userId = GetCurrentUserId();
            try
            {
                var comment = await _eventService.UpdateCommentAsync(commentDto, userId);
                return Ok(comment);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("comments/{id}")]
        public async Task<ActionResult> DeleteComment(Guid id)
        {
            var userId = GetCurrentUserId();
            try
            {
                var result = await _eventService.DeleteCommentAsync(id, userId);
                return result ? NoContent() : NotFound();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
