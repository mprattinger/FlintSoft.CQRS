# FlintSoft.CQRS

A lightweight, opinionated .NET 9 library that provides a clean implementation of the **CQRS (Command Query Responsibility Segregation)** pattern with **Domain-Driven Design (DDD)** support.

## Features

- **Commands & Queries** — clear separation of write and read operations
- **ErrorOr integration** — railway-oriented error handling; no exceptions thrown by default
- **Domain Events** — raise and dispatch events from your entities with multiple handler support
- **Automatic handler discovery** — uses [Scrutor](https://github.com/khellang/Scrutor) to scan and register all handlers in a given assembly
- **Cross-cutting decorators** — built-in logging and exception-handling decorators applied transparently to every handler

## Installation

```bash
dotnet add package FlintSoft.CQRS
```

## Quick Start

### 1. Register the library

Call `AddFlintSoftCQRS` on your `IHostApplicationBuilder`, passing any type from the assembly that contains your handlers:

```csharp
var host = Host.CreateApplicationBuilder(args);

host.AddFlintSoftCQRS(typeof(Program));

var app = host.Build();
```

This single call:
- Scans the assembly for all `ICommandHandler<>` and `IQueryHandler<,>` implementations and registers them as scoped services
- Applies the logging and exception-handling decorators automatically
- Registers `IDomainEventDispatcher` as a transient service

---

### 2. Commands

A **command** represents an intent to change state. Implement `ICommand<TResponse>` for commands that return a value, or `ICommand` for fire-and-forget commands.

```csharp
using FlintSoft.CQRS.Interfaces;
using FlintSoft.CQRS.Handlers;
using ErrorOr;

// Define the command
public record CreateUserCommand(string UserName, string Email) : ICommand<Guid>;

// Implement the handler
public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<ErrorOr<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // ... create the user and persist it ...
        return await Task.FromResult(Guid.NewGuid());
    }
}
```

Execute the command by resolving `ICommandHandler<TCommand, TResponse>` from the DI container:

```csharp
var handler = serviceProvider.GetRequiredService<ICommandHandler<CreateUserCommand, Guid>>();

var result = await handler.Handle(new CreateUserCommand("john", "john@example.com"), cancellationToken);

if (result.IsError)
    Console.WriteLine(result.Errors.First().Description);
else
    Console.WriteLine($"Created user {result.Value}");
```

#### Fire-and-forget command (no return value)

```csharp
public record DeleteUserCommand(Guid UserId) : ICommand;

public sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    public async Task<ErrorOr<Success>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        // ... delete the user ...
        return await Task.FromResult(Result.Success);
    }
}
```

---

### 3. Queries

A **query** retrieves data without changing state. Implement `IQuery<TResponse>`.

```csharp
using FlintSoft.CQRS.Interfaces;
using FlintSoft.CQRS.Handlers;
using ErrorOr;

// Define the query
public record GetUserNameQuery(Guid UserId) : IQuery<string>;

// Implement the handler
public sealed class GetUserNameQueryHandler : IQueryHandler<GetUserNameQuery, string>
{
    public Task<ErrorOr<string>> Handle(GetUserNameQuery query, CancellationToken cancellationToken)
    {
        // ... fetch the user name ...
        return Task.FromResult<ErrorOr<string>>("John Doe");
    }
}
```

Execute the query by resolving `IQueryHandler<TQuery, TResponse>`:

```csharp
var handler = serviceProvider.GetRequiredService<IQueryHandler<GetUserNameQuery, string>>();

var result = await handler.Handle(new GetUserNameQuery(userId), cancellationToken);

Console.WriteLine(result.IsError ? result.Errors.First().Description : result.Value);
```

---

### 4. Domain Events

Domain events allow entities to broadcast side effects in a loosely coupled way.

#### 4.1 Make your entity inherit `DomainEventEntityBase`

```csharp
using FlintSoft.CQRS;

public class User : DomainEventEntityBase
{
    public Guid Id { get; }
    public string UserName { get; }

    public User(Guid id, string userName)
    {
        Id = id;
        UserName = userName;
    }

    public static User Create(string userName)
    {
        var user = new User(Guid.NewGuid(), userName);
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));
        return user;
    }
}
```

#### 4.2 Define a domain event

```csharp
using FlintSoft.CQRS.Events;

public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;
```

#### 4.3 Implement one or more event handlers

Multiple handlers for the same event are all invoked by the dispatcher:

```csharp
using FlintSoft.CQRS.Events;

public sealed class SendWelcomeEmailHandler : IDomainEventHandler<UserCreatedDomainEvent>
{
    public Task Handle(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Sending welcome email for user {domainEvent.UserId}");
        return Task.CompletedTask;
    }
}

public sealed class AuditUserCreatedHandler : IDomainEventHandler<UserCreatedDomainEvent>
{
    public Task Handle(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Auditing creation of user {domainEvent.UserId}");
        return Task.CompletedTask;
    }
}
```

#### 4.4 Dispatch the events

After executing a command, retrieve the events collected on the entity, dispatch them, and then clear the list:

```csharp
var dispatcher = serviceProvider.GetRequiredService<IDomainEventDispatcher>();

// dispatch all events raised during command execution
await dispatcher.DispatchAsync(user.DomainEvents, cancellationToken);

// clear the events once dispatched
user.ClearDomainEvents();
```

---

### 5. Cross-cutting decorators

The following decorators are applied automatically to every command and query handler when you call `AddFlintSoftCQRS`:

| Decorator | Behavior |
|---|---|
| `LoggingDecorator` | Logs start and completion (success or error) of every handler using `ILogger` |
| `ExceptionDecorator` | Catches any unhandled exception and converts it to an `ErrorOr` failure, so callers never receive an unhandled exception |

No additional configuration is required.

---

## Sample Application

A fully-working demo project can be found in [`src/Demo`](src/Demo). It demonstrates:

- Registering `FlintSoft.CQRS` with `AddFlintSoftCQRS`
- A `CreateUser` command with a result type
- A `GetUserName` query
- A `UserCreatedDomainEvent` with two handlers (send email + audit log)
- Dispatching domain events and clearing them after the command

```csharp
// src/Demo/Program.cs
var host = Host.CreateApplicationBuilder(args);

host.AddFlintSoftCQRS(typeof(Program));

var app = host.Build();

var sp = app.Services;
var queryHandler  = sp.GetRequiredService<IQueryHandler<GetUserName.Query, string>>();
var commandHandler = sp.GetRequiredService<ICommandHandler<CreateUser.Command, User>>();
var dispatcher     = sp.GetRequiredService<IDomainEventDispatcher>();

var queryResult = await queryHandler.Handle(new GetUserName.Query(), CancellationToken.None);
Console.WriteLine(queryResult.IsError ? queryResult.Errors.First().Description : queryResult.Value);

var commandResult = await commandHandler.Handle(
    new CreateUser.Command("uname", "geheim", "u@u.com", "John", "Doe"),
    CancellationToken.None);

Console.WriteLine(commandResult.IsError ? commandResult.Errors.First().Description : commandResult.Value);

await dispatcher.DispatchAsync(commandResult.Value.DomainEvents);
commandResult.Value.ClearDomainEvents();

await app.StartAsync();
```

---

## Dependencies

| Package | Version | Purpose |
|---|---|---|
| [ErrorOr](https://github.com/amantinband/error-or) | 2.0.1 | Railway-oriented error handling |
| [Scrutor](https://github.com/khellang/Scrutor) | 7.0.0 | Assembly scanning & decorator registration |
| Microsoft.Extensions.Hosting.Abstractions | 9.0.5 | `IHostApplicationBuilder` DI integration |

## Target Framework

.NET 9.0

## License

See [LICENSE](LICENSE) for details.
