using MassTransit;
using RunMate.User.RunMate.Infrastructure.Messaging.EventBus.Interface;

namespace RunMate.User.RunMate.Infrastructure.Messaging.EventBus
{
    public class MassTransitEventBus : IEventBus
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MassTransitEventBus> _logger;

        public MassTransitEventBus(IPublishEndpoint publishEndpoint, ILogger<MassTransitEventBus> logger)
        {
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishAsync<T>(T @event) where T : class
        {
            _logger.LogInformation($"Publishing event: {@event.GetType().Name}");
            await _publishEndpoint.Publish(@event);
        }
    }
}
