namespace MediatorPattern.Interfaces;

public interface IEventHandler<TEvent> where TEvent : IEvent
{
    public Task Handle(TEvent @event, CancellationToken token = default);
}