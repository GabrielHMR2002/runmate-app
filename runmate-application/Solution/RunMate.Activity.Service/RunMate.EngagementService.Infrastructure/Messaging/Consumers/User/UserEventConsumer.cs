using MassTransit;
using RunMate.EngagementService.Application.Interfaces;
using RunMate.EngagementService.RunMate.EngagementService.Application.DTOs.User;
using RunMate.Shared.Auth.Event.User;

namespace RunMate.EngagementService.RunMate.EngagementService.Infrastructure.Messaging.Consumers.User
{
    public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserCreatedEventConsumer> _logger;

        public UserCreatedEventConsumer(
            IUserService userService,
            ILogger<UserCreatedEventConsumer> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var @event = context.Message;
            _logger.LogInformation("Consuming UserCreatedEvent for user: {UserId}", @event.UserId);

            try
            {
                // Verificar se o usuário já existe (idempotência)
                if (await _userService.UsernameExistsAsync(@event.Username))
                {
                    _logger.LogInformation("User already exists in EngagementService: {Username}", @event.Username);
                    return;
                }

                // Criar um CreateUserDto passando os parâmetros no construtor
                var createUserDto = new CreateUserDto(
                    Username: @event.Username,
                    Password: "MANAGED_BY_AUTH_SERVICE", // Não usamos a senha real aqui
                    Email: @event.Email,
                    FullName: @event.FullName,
                    BirthDate: @event.BirthDate,
                    Role: @event.Role
                );

                // Criar o usuário (sem publicar eventos)
                var result = await _userService.CreateUserWithoutEventAsync(createUserDto);

                if (result.Success)
                {
                    _logger.LogInformation("Successfully created user in EngagementService: {Username}", @event.Username);
                }
                else
                {
                    _logger.LogWarning("Failed to create user in EngagementService: {Message}", result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing UserCreatedEvent for user: {UserId}", @event.UserId);
                // Rethrow para acionar política de retry
                throw;
            }
        }
    }
}