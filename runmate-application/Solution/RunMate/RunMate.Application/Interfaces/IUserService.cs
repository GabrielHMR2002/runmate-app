using RunMate.RunMate.Application.DTOs.UserDTOs;
using RunMate.User.RunMate.Application.DTOs.UserDTOs;

namespace RunMate.RunMate.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(Guid id);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<(bool Success, string Message, Guid UserId)> CreateUserAsync(RegisterUserDto userDto);
        Task<(bool Success, string Message)> UpdateUserAsync(Guid id, UpdateUserDto userDto);
        Task<(bool Success, string Message)> DeleteUserAsync(Guid id);
        Task<(bool Success, string Message)> ChangePasswordAsync(Guid id, ChangePasswordDto passwordDto);
        Task<(bool Success, string Message)> ToggleUserStatusAsync(Guid id);
    }
}
