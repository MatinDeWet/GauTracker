using Microsoft.Extensions.Logging;

namespace Worker.Application.Logging;

internal static partial class WorkerLog
{
    [LoggerMessage(Level = LogLevel.Information, Message = "ExampleJob ran. There are {CardCount} card(s) in the database.")]
    public static partial void ExampleJobRan(this ILogger logger, int cardCount);
}
