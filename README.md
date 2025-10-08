# MediatorPattern

A simple implementation of the Mediator Pattern supporting Publisher/Subscriber and CQRS models.

## Installation

```bash
dotnet add package MediatorPattern
```

## Setup

Register the mediator in your `Program.cs` or `Startup.cs`:

```csharp
builder.Services.AddMediator();
```

## Usage

### Publisher / Subscriber Example

Define your event and its handler:

```csharp
public class CustomEvent : IEvent { }

public class CustomEventHandler : IEventHandler<CustomEvent>
{
    public Task Handle(CustomEvent @event, CancellationToken token)
    {
        // Handle the event logic here
        return Task.CompletedTask;
    }
}
```

Raise the event:

```csharp
await mediator.Raise(new CustomEvent());
```

### CQRS Example

Define your action and its handler:

```csharp
public class CustomAction : IAction<ReturnType>
{
    // Add any properties needed for execution
}

public class CustomActionHandler : IActionHandler<CustomAction, ReturnType>
{
    public Task<ReturnType> Handle(CustomAction action, CancellationToken token)
    {
        // Execute your logic and return a result
        return Task.FromResult(new ReturnType());
    }
}
```

Execute the action:

```csharp
var result = await mediator.Execute(new CustomAction());
```

## Summary

✅ Lightweight and easy to use  
⚡ Supports Publisher/Subscriber pattern  
🧩 Built-in CQRS support  
