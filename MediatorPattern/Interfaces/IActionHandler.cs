namespace MediatorPattern.Interfaces;

public interface IActionHandler<TAction, TReturn> where TAction : IAction<TReturn>
{
    public Task<TReturn> Handle(TAction transaction, CancellationToken token = default);
}