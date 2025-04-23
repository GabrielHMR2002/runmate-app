using RunMate.EngagementService.RunMate.EngagementService.Application.DTOs.User;

namespace RunMate.EngagementService.Application.Interfaces
{
    public interface IUserService
    {
        // Métodos de consulta
        Task<UserResponseDto> GetUserByIdAsync(Guid id);
        Task<UserResponseDto> GetUserByUsernameAsync(string username);

        // Métodos de verificação
        Task<bool> ExistsAsync(Guid id);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);

        // Método de criação especial para o consumidor de eventos
        Task<UserResponseDto> CreateUserWithoutEventAsync(CreateUserDto createUserDto);

        // Métodos para atualizações locais (sem publicar eventos)
        Task<UserResponseDto> UpdateUserWithoutEventAsync(Guid id, UpdateUserDto updateUserDto);
        Task<UserResponseDto> ActivateUserWithoutEventAsync(Guid id);
        Task<UserResponseDto> DeactivateUserWithoutEventAsync(Guid id);
        Task<bool> DeleteUserWithoutEventAsync(Guid id);
    }
}