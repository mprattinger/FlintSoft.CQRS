using FlintSoft.CQRS;

namespace Demo.Features.User.Queries;

public static class GetUserName
{
    public record Query() : IRequest<string>;

    internal sealed class Handler : IRequestHandler<Query, string>
    {
        public async Task<string> Handle(Query request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(Environment.UserName);
        }
    }
}
