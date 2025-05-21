using ErrorOr;

namespace FlintSoft.CQRS;

public static class ExceptionDecorator
{
    public sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler
    )
    : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<ErrorOr<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).FullName ?? typeof(TCommand).Name;

            try
            {
                return await innerHandler.Handle(command, cancellationToken);
            }
            catch (Exception ex)
            {
                return Error.Failure(commandName.ToUpper(), $"Error executing command {commandName}: {ex.Message}");
            }
        }
    }

    public sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler
    )
    : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<ErrorOr<Success>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).FullName ?? typeof(TCommand).Name;

            try
            {
                return await innerHandler.Handle(command, cancellationToken);
            }
            catch (Exception ex)
            {
                return Error.Failure(commandName.ToUpper(), $"Error executing command {commandName}: {ex.Message}");
            }
        }
    }

    public sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler
    )
    : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<ErrorOr<TResponse>> Handle(TQuery command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TQuery).FullName ?? typeof(TQuery).Name;

            try
            {
                return await innerHandler.Handle(command, cancellationToken);
            }
            catch (Exception ex)
            {
                return Error.Failure(commandName.ToUpper(), $"Error executing query {commandName}: {ex.Message}");
            }
        }
    }
}
