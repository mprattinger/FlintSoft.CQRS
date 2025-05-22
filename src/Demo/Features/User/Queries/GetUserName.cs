using ErrorOr;
using FlintSoft.CQRS.Handlers;
using FlintSoft.CQRS.Interfaces;

namespace Demo.Features.User.Queries;

public static class GetUserName
{
    public record Query() : IQuery<string>;

    internal sealed class Handler : IQueryHandler<Query, string>
    {
        public async Task<ErrorOr<string>> Handle(Query query, CancellationToken cancellationToken)
        {
            return await Task.FromResult(Environment.UserName);
        }
    }
}
