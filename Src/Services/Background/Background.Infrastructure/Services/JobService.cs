using System.ComponentModel;
using Ardalis.Result;
using CQRS.Base;
using CQRS.Core.Contracts;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Background.Infrastructure.Services;
internal sealed class JobService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<JobService> _logger;

    public JobService(IServiceProvider serviceProvider, ILogger<JobService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    [Queue("default")]
    [AutomaticRetry(Attempts = 3, DelaysInSeconds = [60, 300, 900])]
    [DisplayName("Execute Command: {0}")]
    public async Task ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        string commandName = typeof(TCommand).Name;
        _logger.LogInformation("Starting execution of command: {CommandName}", commandName);

        try
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            ICommandManager<TCommand> handler = scope.ServiceProvider.GetRequiredService<ICommandManager<TCommand>>();

            Result result = await handler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully completed command: {CommandName}", commandName);
            }
            else
            {
                _logger.LogError("Command {CommandName} failed: {Errors}", commandName, result.Errors);

                throw new InvalidOperationException($"Command {commandName} failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during command execution: {CommandName}", commandName);
            throw;
        }
    }
}
