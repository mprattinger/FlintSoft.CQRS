using ErrorOr;
using FlintSoft.CQRS;

namespace Demo.Features.User.Commands;

public static class CreateUser
{
    public record Command(string UserName) : ICommand<Guid>;

    internal sealed class Handler : ICommandHandler<Command, Guid>
    {
        public async Task<ErrorOr<Guid>> Handle(Command command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("This is a stub implementation. Replace with actual logic.");
            return await Task.FromResult(Guid.NewGuid());
        }
    }
}
