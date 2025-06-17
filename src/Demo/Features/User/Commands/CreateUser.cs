using Demo.Features.User.Events;
using ErrorOr;
using FlintSoft.CQRS.Handlers;
using FlintSoft.CQRS.Interfaces;

namespace Demo.Features.User.Commands;

public static class CreateUser
{
    public record Command(string UserName, string Password, string Email, string FirstName, string LastName) : ICommand<User>;

    internal sealed class Handler : ICommandHandler<Command, User>
    {
        public async Task<ErrorOr<User>> Handle(Command command, CancellationToken cancellationToken)
        {
            var user = new User(
                command.UserName,
                command.Password,
                command.Email,
                command.FirstName,
                command.LastName
            );

            // Here you would typically save the user to a database or some other storage.

            user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

            return await Task.FromResult(user);
        }
    }
}
