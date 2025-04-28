using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RunMate.UserService.RunMate.Application.DTOs.Publication;
using RunMate.UserService.RunMate.Application.Interfaces;
using System.Security.Claims;

namespace RunMate.UserService.RunMate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PublicationsController : ControllerBase
    {
        private readonly IPublicationService _publicationService;

        public PublicationsController(IPublicationService publicationService)
        {
            _publicationService = publicationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFeed([FromQuery] FeedParamsDto parameters)
        {
            var currentUserId = GetCurrentUserId();
            var feed = await _publicationService.GetFeedAsync(parameters, currentUserId);
            return Ok(feed);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            var publication = await _publicationService.GetByIdAsync(id, currentUserId);

            if (publication == null)
                return NotFound();

            return Ok(publication);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePublicationDto dto)
        {
            var currentUserId = GetCurrentUserId();
            var publication = await _publicationService.CreateAsync(dto, currentUserId);
            return CreatedAtAction(nameof(GetById), new { id = publication.Id }, publication);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePublicationDto dto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var publication = await _publicationService.UpdateAsync(id, dto, currentUserId);
                return Ok(publication);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                await _publicationService.DeleteAsync(id, currentUserId);
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

        [HttpPost("{id}/like")]
        public async Task<IActionResult> Like(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var result = await _publicationService.LikeAsync(id, currentUserId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}/like")]
        public async Task<IActionResult> Unlike(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var result = await _publicationService.UnlikeAsync(id, currentUserId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetComments(Guid id, [FromQuery] CommentListParamsDto parameters)
        {
            try
            {
                var comments = await _publicationService.GetCommentsAsync(id, parameters);
                return Ok(comments);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddComment(Guid id, [FromBody] PublicationCommentDto dto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var comment = await _publicationService.AddCommentAsync(id, dto, currentUserId);
                return CreatedAtAction(nameof(GetComments), new { id }, comment);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
