using RunMate.Authentication.RunMate.Application.DTOs.LoginDTOs;
using RunMate.Domain.Entities;

namespace RunMate.RunMate.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Login(LoginRequestDto request);
        string GenerateJwtToken(UserEntity user);
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}
