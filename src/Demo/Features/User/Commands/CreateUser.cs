using ErrorOr;
using FlintSoft.CQRS.Handlers;
using FlintSoft.CQRS.Interfaces;
using FluentValidation;

namespace Demo.Features.User.Commands;

public static class CreateUser
{
    public record Command(string UserName) : ICommand<Guid>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserName).NotEmpty();
        }
    }

    internal sealed class Handler : ICommandHandler<Command, Guid>
    {
        public async Task<ErrorOr<Guid>> Handle(Command command, CancellationToken cancellationToken)
        {
            if (command.UserName == "error")
            {
                throw new NotImplementedException("Error creating the user");
            }

            if (command.UserName == "wrong")
            {
                return Error.Failure("USER_CREATE_FAILED", $"Failure creating user!");
            }

            return await Task.FromResult(Guid.NewGuid());
        }
    }
}
