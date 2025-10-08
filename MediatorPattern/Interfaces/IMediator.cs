namespace MediatorPattern.Interfaces;

public interface IMediator
{
    public Task Raise(IEvent @event, CancellationToken token = default);
    
    public Task<T?> Execute<T>(IAction<T> action, CancellationToken token = default);
}