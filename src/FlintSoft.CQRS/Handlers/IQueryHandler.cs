using ErrorOr;
using FlintSoft.CQRS.Interfaces;

namespace FlintSoft.CQRS.Handlers;

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<ErrorOr<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}

