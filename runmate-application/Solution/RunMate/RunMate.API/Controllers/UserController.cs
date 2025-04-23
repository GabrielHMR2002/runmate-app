using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RunMate.RunMate.Application.DTOs.UserDTOs;
using RunMate.RunMate.Application.Interfaces;
using RunMate.User.RunMate.API.Common.ApiResponse;
using RunMate.User.RunMate.Application.DTOs.UserDTOs;
using System.Security.Claims;

namespace RunMate.RunMate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class UsersController(IUserService userService, ILogger<UsersController> logger) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly ILogger<UsersController> _logger = logger;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<object>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new ApiResponse<object>(users));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(Guid id)
        {
            if (!await AuthorizeUserAccessOrAdmin(id))
            {
                return Forbid();
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse("Usuário não encontrado"));
            }

            return Ok(ApiResponse<UserDto>.SuccessResponse(user));
        }

        [HttpGet("username/{username}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserByUsername(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse("Usuário não encontrado"));
            }

            return Ok(ApiResponse<UserDto>.SuccessResponse(user));
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateUser([FromBody] RegisterUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<Guid>.ErrorResponse("Dados inválidos", ModelState));
            }

            var (success, message, userId) = await _userService.CreateUserAsync(userDto);
            if (!success)
            {
                return BadRequest(ApiResponse<Guid>.ErrorResponse(message));
            }

            _logger.LogInformation("Usuário criado: {Username}", userDto.Username);
            return CreatedAtAction(
                nameof(GetUserById),
                new { id = userId },
                ApiResponse<Guid>.SuccessResponse(userId, message));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Dados inválidos", ModelState));
            }

            if (!await AuthorizeUserAccessOrAdmin(id))
            {
                return Forbid();
            }

            // Se não for admin, não pode alterar o Role ou IsActive
            if (!User.IsInRole("Admin"))
            {
                userDto.Role = null;
                userDto.IsActive = null;
            }

            var (success, message) = await _userService.UpdateUserAsync(id, userDto);
            if (!success)
            {
                if (message.Contains("não encontrado"))
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse(message));
                }

                return BadRequest(ApiResponse<bool>.ErrorResponse(message));
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            var (success, message) = await _userService.DeleteUserAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse(message));
            }

            return NoContent();
        }

        [HttpPut("{id}/change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto passwordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Dados inválidos", ModelState));
            }

            // Verificar se o usuário está tentando alterar sua própria senha
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != id.ToString())
            {
                return Forbid();
            }

            var (success, message) = await _userService.ChangePasswordAsync(id, passwordDto);
            if (!success)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(message));
            }

            return NoContent();
        }

        [HttpPut("{id}/toggle-status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ToggleUserStatus(Guid id)
        {
            var (success, message) = await _userService.ToggleUserStatusAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse(message));
            }

            return NoContent();
        }

        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out var userId))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse("Usuário não encontrado"));
            }

            return Ok(ApiResponse<UserDto>.SuccessResponse(user));
        }

        private async Task<bool> AuthorizeUserAccessOrAdmin(Guid userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            return isAdmin || (currentUserId == userId.ToString());
        }
    }
}