using FlintSoft.CQRS.Events;

namespace Demo.Features.User.Events;

public sealed record UserCreatedDomainEvent(Guid userId) : IDomainEvent;