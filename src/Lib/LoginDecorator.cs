using System;
using ErrorOr;
using FlintSoft.CQRS;
using Microsoft.Extensions.Logging;

namespace Lib;

public static class LoggingDecorator
{
    public sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger
    ) : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<ErrorOr<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).FullName ?? typeof(TCommand).Name;

            logger.LogInformation("Processing command {Command}", commandName);

            var result = await innerHandler.Handle(command, cancellationToken);

            if (!result.IsError)
            {
                logger.LogInformation("Completed command {Command}", commandName);
            }
            else
            {
                logger.LogError("Completed command {Command} with error", commandName);
                // using (LogContext.PushProperty("Error", result.Error, true))
                // {
                //     logger.LogError("Completed command {Command} with error", commandName);
                // }
            }

            return result;
        }
    }

    public sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        ILogger<CommandBaseHandler<TCommand>> logger
    ) : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<ErrorOr<Success>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).FullName ?? typeof(TCommand).Name;

            logger.LogInformation("Processing command {Command}", commandName);

            var result = await innerHandler.Handle(command, cancellationToken);

            if (!result.IsError)
            {
                logger.LogInformation("Completed command {Command}", commandName);
            }
            else
            {
                logger.LogError("Completed command {Command} with error", commandName);
                // using (LogContext.PushProperty("Error", result.Error, true))
                // {
                //     logger.LogError("Completed command {Command} with error", commandName);
                // }
            }

            return result;
        }
    }

    public sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger
    ) : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<ErrorOr<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            string queryName = typeof(TQuery).FullName ?? typeof(TQuery).Name; ;

            logger.LogInformation("Processing query {Query}", queryName);

            var result = await innerHandler.Handle(query, cancellationToken);

            if (!result.IsError)
            {
                logger.LogInformation("Completed query {Query}", queryName);
            }
            else
            {
                logger.LogError("Completed query {Query} with error", queryName);
                // using (LogContext.PushProperty("Error", result.Error, true))
                // {
                //     logger.LogError("Completed query {Query} with error", queryName);
                // }
            }

            return result;
        }
    }
}
