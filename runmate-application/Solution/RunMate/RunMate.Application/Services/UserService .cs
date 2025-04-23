using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RunMate.Authentication.RunMate.Infrastructure.Persistence.Context;
using RunMate.Domain.Entities;
using RunMate.RunMate.Application.DTOs.UserDTOs;
using RunMate.RunMate.Application.Interfaces;
using RunMate.User.RunMate.Application.DTOs.UserDTOs;
using RunMate.User.RunMate.Infrastructure.Messaging.EventBus.Interface;
using RunMate.Shared.Auth.Event.User;

namespace RunMate.RunMate.Application.Services
{
    public class UserService : IUserService
    {
        private readonly RunMateContext _context;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IEventBus _eventBus;

        public UserService(
            RunMateContext context,
            IAuthService authService,
            IMapper mapper,
            ILogger<UserService> logger,
            IEventBus eventBus)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<(bool Success, string Message, Guid UserId)> CreateUserAsync(RegisterUserDto userDto)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
                {
                    return (false, "Nome de usuário já existe", Guid.Empty);
                }

                if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
                {
                    return (false, "Email já está em uso", Guid.Empty);
                }

                // Use o tipo exato que seu DbContext espera
                var user = new UserEntity
                {
                    Id = Guid.NewGuid(),
                    Username = userDto.Username,
                    Email = userDto.Email,
                    FullName = userDto.FullName,
                    BirthDate = userDto.BirthDate,
                    PasswordHash = _authService.HashPassword(userDto.Password),
                    Role = userDto.Role,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuário criado: {Id}, {Username}", user.Id, user.Username);

                // Publicar evento de criação de usuário
                await PublishUserCreatedEventAsync(user);

                return (true, "Usuário criado com sucesso", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário {Username}", userDto.Username);
                return (false, $"Erro ao criar usuário: {ex.Message}", Guid.Empty);
            }
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(Guid id, UpdateUserDto userDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return (false, "Usuário não encontrado");
                }

                if (!string.IsNullOrEmpty(userDto.Email) && userDto.Email != user.Email)
                {
                    if (await _context.Users.AnyAsync(u => u.Email == userDto.Email && u.Id != id))
                    {
                        return (false, "Email já está em uso");
                    }
                    user.Email = userDto.Email;
                }

                if (!string.IsNullOrEmpty(userDto.FullName))
                {
                    user.FullName = userDto.FullName;
                }

                if (userDto.BirthDate.HasValue)
                {
                    user.BirthDate = userDto.BirthDate.Value;
                }

                if (userDto.Role.HasValue)
                {
                    user.Role = userDto.Role.Value;
                }

                if (userDto.IsActive.HasValue)
                {
                    user.IsActive = userDto.IsActive.Value;
                }

                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Usuário atualizado: {Id}", id);

                // Publicar evento de atualização
                await PublishUserUpdatedEventAsync(user);

                return (true, "Usuário atualizado com sucesso");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao atualizar usuário {Id}", id);
                return (false, $"Erro ao atualizar usuário: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(Guid id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return (false, "Usuário não encontrado");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Usuário excluído: {Id}", id);

                // Publicar evento de exclusão
                await PublishUserDeletedEventAsync(id);

                return (true, "Usuário excluído com sucesso");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao excluir usuário {Id}", id);
                return (false, $"Erro ao excluir usuário: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(Guid id, ChangePasswordDto passwordDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return (false, "Usuário não encontrado");
                }

                if (!_authService.VerifyPassword(passwordDto.CurrentPassword, user.PasswordHash))
                {
                    _logger.LogWarning("Tentativa de alteração de senha com senha atual incorreta: {Id}", id);
                    return (false, "Senha atual incorreta");
                }

                user.PasswordHash = _authService.HashPassword(passwordDto.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Senha alterada para usuário: {Id}", id);

                // Publicar evento de alteração de senha (se necessário)
                // await PublishUserPasswordChangedEventAsync(user);

                return (true, "Senha alterada com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar senha do usuário {Id}", id);
                return (false, $"Erro ao alterar senha: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> ToggleUserStatusAsync(Guid id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return (false, "Usuário não encontrado");
                }

                user.IsActive = !user.IsActive;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Status do usuário alterado: {Id}, Ativo: {IsActive}", id, user.IsActive);

                // Publicar evento de mudança de status
                await PublishUserStatusChangedEventAsync(user);

                return (true, $"Status do usuário alterado para {(user.IsActive ? "ativo" : "inativo")}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alternar status do usuário {Id}", id);
                return (false, $"Erro ao alterar status: {ex.Message}");
            }
        }

        #region Event Publishing Methods

        private async Task PublishUserCreatedEventAsync(UserEntity user)
        {
            try
            {
                var @event = new UserCreatedEvent
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    PasswordHash = user.PasswordHash,
                    FullName = user.FullName ?? string.Empty,
                    BirthDate = user.BirthDate,
                    Role = user.Role.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = user.IsActive
                };

                _logger.LogInformation("Publishing UserCreatedEvent for user: {UserId}", user.Id);
                await _eventBus.PublishAsync(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing UserCreatedEvent for user: {UserId}", user.Id);
                // Não lançamos a exceção novamente para não interromper o fluxo principal
            }
        }

        private async Task PublishUserUpdatedEventAsync(UserEntity user)
        {
            try
            {
                var @event = new UserUpdatedEvent
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName ?? string.Empty,
                    BirthDate = user.BirthDate,
                    Role = user.Role.ToString(),
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = user.IsActive
                };

                _logger.LogInformation("Publishing UserUpdatedEvent for user: {UserId}", user.Id);
                await _eventBus.PublishAsync(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing UserUpdatedEvent for user: {UserId}", user.Id);
            }
        }

        private async Task PublishUserDeletedEventAsync(Guid userId)
        {
            try
            {
                var @event = new UserDeletedEvent
                {
                    UserId = userId,
                    DeletedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Publishing UserDeletedEvent for user: {UserId}", userId);
                await _eventBus.PublishAsync(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing UserDeletedEvent for user: {UserId}", userId);
            }
        }

        private async Task PublishUserStatusChangedEventAsync(UserEntity user)
        {
            try
            {
                var @event = new UserStatusChangedEvent
                {
                    UserId = user.Id,
                    IsActive = user.IsActive,
                    UpdatedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Publishing UserStatusChangedEvent for user: {UserId}", user.Id);
                await _eventBus.PublishAsync(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing UserStatusChangedEvent for user: {UserId}", user.Id);
            }
        }

        #endregion
    }
}