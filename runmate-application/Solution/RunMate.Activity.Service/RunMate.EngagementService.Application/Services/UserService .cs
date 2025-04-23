using AutoMapper;
using RunMate.EngagementService.Application.Interfaces;
using RunMate.EngagementService.Domain.Models;
using RunMate.EngagementService.Infrastructure.Persistence.Repositories;
using RunMate.EngagementService.RunMate.EngagementService.Application.DTOs.User;
using RunMate.EngagementService.RunMate.EngagementService.Domain.Enums;

namespace RunMate.EngagementService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserResponseDto> GetUserByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting user by ID: {UserId}", id);

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", id);
                throw new ArgumentException($"User with ID {id} not found.", nameof(id));
            }

            var userDto = _mapper.Map<UserDto>(user);

            return new UserResponseDto(
                Success: true,
                Message: "User found successfully",
                User: userDto
            );
        }

        public async Task<UserResponseDto> GetUserByUsernameAsync(string username)
        {
            _logger.LogInformation("Getting user by username: {Username}", username);

            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null)
            {
                _logger.LogWarning("User not found with username: {Username}", username);
                throw new ArgumentException($"User with username '{username}' not found.", nameof(username));
            }

            var userDto = _mapper.Map<UserDto>(user);

            return new UserResponseDto(
                Success: true,
                Message: "User found successfully",
                User: userDto
            );
        }

        public async Task<UserResponseDto> CreateUserWithoutEventAsync(CreateUserDto createUserDto)
        {
            _logger.LogInformation("Creating user from event with username: {Username}", createUserDto.Username);

            // Check if username already exists
            if (await _userRepository.ExistsByUsernameAsync(createUserDto.Username))
            {
                _logger.LogWarning("Username already exists: {Username}", createUserDto.Username);
                throw new InvalidOperationException($"Username '{createUserDto.Username}' already exists.");
            }

            // Check if email already exists
            if (await _userRepository.ExistsByEmailAsync(createUserDto.Email))
            {
                _logger.LogWarning("Email already exists: {Email}", createUserDto.Email);
                throw new InvalidOperationException($"Email '{createUserDto.Email}' already exists.");
            }

            // Determine user role
            var role = UserRole.User;
            if (!string.IsNullOrEmpty(createUserDto.Role) &&
                Enum.TryParse<UserRole>(createUserDto.Role, true, out var parsedRole))
            {
                role = parsedRole;
            }

            // Create user with placeholder password
            var user = new User(
                username: createUserDto.Username,
                passwordHash: "MANAGED_BY_AUTH_SERVICE", // Placeholder since auth is handled by RunMate.User
                email: createUserDto.Email,
                fullName: createUserDto.FullName,
                birthDate: createUserDto.BirthDate,
                role: role
            );

            // Save user
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("User created from event successfully. ID: {UserId}", user.Id);

            // Map to DTO and return
            var userDto = _mapper.Map<UserDto>(user);

            return new UserResponseDto(
                Success: true,
                Message: "User created successfully",
                User: userDto
            );
        }

        public async Task<UserResponseDto> UpdateUserWithoutEventAsync(Guid id, UpdateUserDto updateUserDto)
        {
            _logger.LogInformation("Updating user with ID: {UserId}", id);

            // Get user
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", id);
                throw new ArgumentException($"User with ID {id} not found.", nameof(id));
            }

            // Check if email already exists (if being changed)
            if (!string.IsNullOrEmpty(updateUserDto.Email) &&
                updateUserDto.Email != user.Email &&
                await _userRepository.ExistsByEmailAsync(updateUserDto.Email))
            {
                _logger.LogWarning("Email already exists: {Email}", updateUserDto.Email);
                throw new InvalidOperationException($"Email '{updateUserDto.Email}' already exists.");
            }

            // Determine user role (if being changed)
            UserRole? role = null;
            if (!string.IsNullOrEmpty(updateUserDto.Role) &&
                Enum.TryParse<UserRole>(updateUserDto.Role, true, out var parsedRole))
            {
                role = parsedRole;
            }

            // Update user
            user.Update(
                email: updateUserDto.Email,
                fullName: updateUserDto.FullName,
                birthDate: updateUserDto.BirthDate,
                role: role
            );

            // Save changes
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("User updated successfully. ID: {UserId}", id);

            // Map to DTO and return
            var userDto = _mapper.Map<UserDto>(user);

            return new UserResponseDto(
                Success: true,
                Message: "User updated successfully",
                User: userDto
            );
        }

        public async Task<UserResponseDto> ActivateUserWithoutEventAsync(Guid id)
        {
            _logger.LogInformation("Activating user with ID: {UserId}", id);

            // Get user
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", id);
                throw new ArgumentException($"User with ID {id} not found.", nameof(id));
            }

            // Activate user
            user.Activate();

            // Save changes
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("User activated successfully. ID: {UserId}", id);

            // Map to DTO and return
            var userDto = _mapper.Map<UserDto>(user);

            return new UserResponseDto(
                Success: true,
                Message: "User activated successfully",
                User: userDto
            );
        }

        public async Task<UserResponseDto> DeactivateUserWithoutEventAsync(Guid id)
        {
            _logger.LogInformation("Deactivating user with ID: {UserId}", id);

            // Get user
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", id);
                throw new ArgumentException($"User with ID {id} not found.", nameof(id));
            }

            // Deactivate user
            user.Deactivate();

            // Save changes
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("User deactivated successfully. ID: {UserId}", id);

            // Map to DTO and return
            var userDto = _mapper.Map<UserDto>(user);

            return new UserResponseDto(
                Success: true,
                Message: "User deactivated successfully",
                User: userDto
            );
        }

        public async Task<bool> DeleteUserWithoutEventAsync(Guid id)
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);

            // Get user
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId} for deletion", id);
                return false;
            }

            // Delete user
            await _userRepository.DeleteAsync(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("User deleted successfully. ID: {UserId}", id);

            return true;
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            return _userRepository.ExistsByIdAsync(id);
        }

        public Task<bool> UsernameExistsAsync(string username)
        {
            return _userRepository.ExistsByUsernameAsync(username);
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            return _userRepository.ExistsByEmailAsync(email);
        }
    }
}