using FlintSoft.CQRS;

namespace Demo.Features.User.Queries;

public static class GetUserName
{
    public record Query() : IRequest<string>;

    internal sealed class Handler : IRequestHandler<Query, string>
    {
        public string Handle(Query request)
        {
            return Environment.UserName;
        }
    }
}
