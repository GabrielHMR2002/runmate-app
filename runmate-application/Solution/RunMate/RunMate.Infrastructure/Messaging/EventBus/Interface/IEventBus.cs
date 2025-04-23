namespace RunMate.User.RunMate.Infrastructure.Messaging.EventBus.Interface
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T @event) where T : class;
    }
}
