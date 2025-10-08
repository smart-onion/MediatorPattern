using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using MediatorPattern.Interfaces;

namespace MediatorPattern;

public class Mediator : IMediator
{
    private ConcurrentDictionary<Type, Type?> _events = new();

    private ConcurrentDictionary<Type, Type?> _actions = new();

    public Mediator(Assembly assembly)
    {
        RegisterEvents(assembly);
        RegisterActions(assembly);
    }

    private void RegisterEvents(Assembly assembly)
    {
        var events = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IEvent).IsAssignableFrom(t)).ToList();

        var eventHandlers = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract &&
                        t.GetInterfaces().Any(i =>
                            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
            ).ToList();

        var ev = events.ToDictionary(e => e,
            v => eventHandlers.SingleOrDefault(h => h.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>) &&
                i.GenericTypeArguments[0] == v)));
        _events = new(ev);
    }


    private void RegisterActions(Assembly assembly)
    {
        var actions = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract &&
                        t.GetInterfaces().Any(i =>
                            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAction<>))).ToList();
        var actionHandlers = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract &&
                        t.GetInterfaces()
                            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IActionHandler<,>))
            ).ToList();

        var dict = actions.ToDictionary(k => k, v => actionHandlers.SingleOrDefault(h =>
            h.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IActionHandler<,>) &&
                i.GetGenericArguments()[0] == v)
        ));
        _actions = new(dict);
    }

    public Task Raise(IEvent @event, CancellationToken cancellationToken = default)
    {
        var eventType = @event.GetType();

        if (_events.TryGetValue(eventType, out var handlerType) && handlerType != null)
        {
            var handler = Activator.CreateInstance(handlerType);
            var method = handlerType.GetMethod(nameof(IEventHandler<IEvent>.Handle));
            if (method != null)
            {
                return (Task)method.Invoke(handler, new object?[] { @event, cancellationToken })!;
            }
        }

        return Task.CompletedTask;
    }


    public Task<T?> Execute<T>(IAction<T> action, CancellationToken cancellationToken = default)
    {
        var actionType = action.GetType();

        if (_actions.TryGetValue(actionType, out var handlerType) && handlerType != null)
        {
            var handler = Activator.CreateInstance(handlerType);
            var method = handlerType.GetMethod(nameof(IActionHandler<IAction<T>, T>.Handle));
            if (method != null)
            {
                return (Task<T?>)method.Invoke(handler, new object?[] { action, cancellationToken })!;
            }
        }

        return Task.FromResult(default(T));
    }
}