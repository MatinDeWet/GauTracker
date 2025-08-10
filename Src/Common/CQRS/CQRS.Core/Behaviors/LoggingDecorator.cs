using System.Diagnostics;
using Ardalis.Result;
using CQRS.Base;
using CQRS.Core.Contracts;
using Microsoft.Extensions.Logging;

namespace CQRS.Core.Behaviors;
internal static class LoggingDecorator
{
    internal sealed class CommandManager<TCommand, TResponse>(
        ICommandManager<TCommand, TResponse> innerHandler,
        ILogger<CommandManager<TCommand, TResponse>> logger)
        : ICommandManager<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var timer = new Stopwatch();
            timer.Start();

            Result<TResponse> result = await innerHandler.Handle(command, cancellationToken);

            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;

            if (timeTaken.Seconds > 3) // if the request is greater than 3 seconds, then log the warnings
            {
                logger.LogWarning(
                    "[PERFORMANCE] The request {Request} took {TimeTaken} seconds. with data = {RequestData}",
                    typeof(TCommand).Name,
                    timeTaken.Seconds,
                    command
                );
            }

            return result;
        }
    }

    internal sealed class CommandBaseManager<TCommand>(
        ICommandManager<TCommand> innerHandler,
        ILogger<CommandBaseManager<TCommand>> logger)
        : ICommandManager<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var timer = new Stopwatch();
            timer.Start();

            Result result = await innerHandler.Handle(command, cancellationToken);

            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;

            if (timeTaken.Seconds > 3) // if the request is greater than 3 seconds, then log the warnings
            {
                logger.LogWarning(
                    "[PERFORMANCE] The request {Request} took {TimeTaken} seconds. with data = {RequestData}",
                    typeof(TCommand).Name,
                    timeTaken.Seconds,
                    command
                );
            }

            return result;
        }
    }

    internal sealed class QueryManager<TQuery, TResponse>(
        IQueryManager<TQuery, TResponse> innerHandler,
        ILogger<QueryManager<TQuery, TResponse>> logger)
        : IQueryManager<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            var timer = new Stopwatch();
            timer.Start();

            Result<TResponse> result = await innerHandler.Handle(query, cancellationToken);

            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;

            if (timeTaken.Seconds > 3) // if the request is greater than 3 seconds, then log the warnings
            {
                logger.LogWarning(
                    "[PERFORMANCE] The request {Request} took {TimeTaken} seconds. with data = {RequestData}",
                    typeof(TQuery).Name,
                    timeTaken.Seconds,
                    query
                );
            }

            return result;
        }
    }
}
