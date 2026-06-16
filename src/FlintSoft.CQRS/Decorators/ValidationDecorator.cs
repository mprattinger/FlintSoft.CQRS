using ErrorOr;
using FlintSoft.CQRS.Handlers;
using FlintSoft.CQRS.Interfaces;
using FluentValidation;

namespace FlintSoft.CQRS.Decorators;

public static class ValidationDecorator
{
    public sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        IEnumerable<IValidator<TCommand>> validators
    ) : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<ErrorOr<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var errors = validators
                .Select(v => v.Validate(command))
                .SelectMany(result => result.Errors)
                .Where(failure => failure != null)
                .ToList();
            if (errors.Count != 0)
            {
                return Error.Validation(description: string.Join(", ", errors.Select(e => e.ErrorMessage)));
            }

            var result = await innerHandler.Handle(command, cancellationToken);

            return result;
        }
    }

    public sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        IEnumerable<IValidator<TCommand>> validators
    ) : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<ErrorOr<Success>> Handle(TCommand command, CancellationToken cancellationToken)
        {

            var result = await innerHandler.Handle(command, cancellationToken);

            var errors = validators
                .Select(v => v.Validate(command))
                .SelectMany(result => result.Errors)
                .Where(failure => failure != null)
                .ToList();
            if (errors.Count != 0)
            {
                return Error.Validation(description: string.Join(", ", errors.Select(e => e.ErrorMessage)));
            }

            return result;
        }
    }

    public sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler
    ) : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<ErrorOr<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {

            var result = await innerHandler.Handle(query, cancellationToken);

            return result;
        }
    }
}
