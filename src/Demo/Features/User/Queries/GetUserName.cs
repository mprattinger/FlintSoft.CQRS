using ErrorOr;
using FlintSoft.CQRS.Handlers;
using FlintSoft.CQRS.Interfaces;

namespace Demo.Features.User.Queries;

public static class GetUserName
{
    public record Query() : IQuery<string>;

    internal sealed class Handler : IQueryHandler<Query, string>
    {
        public Task<ErrorOr<string>> Handle(Query query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("This is a stub implementation. Replace with actual logic.");
            //return await Task.FromResult(Environment.UserName);
        }
    }
}
