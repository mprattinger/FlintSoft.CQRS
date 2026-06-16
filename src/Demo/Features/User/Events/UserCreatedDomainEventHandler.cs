using FlintSoft.CQRS.Events;

namespace Demo.Features.User.Events;

internal sealed class UserCreatedDomainEventHandler : IDomainEventHandler<UserCreatedDomainEvent>
{
    public Task Handle(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        return Task.CompletedTask; // No operation needed for this event handler
    }
}

internal sealed class UserCreatedDomainEventHandler1 : IDomainEventHandler<UserCreatedDomainEvent>
{
    public Task Handle(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine($"User with guid {domainEvent.userId} created -> send email");
        return Task.CompletedTask; // No operation needed for this event handler
    }
}