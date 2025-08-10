using Background.Application.Services;
using CQRS.Base;
using Hangfire;

namespace Background.Infrastructure.Services;
internal sealed class JobSchedulerService : IJobSchedulerService
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IRecurringJobManager _recurringJobManager;

    public JobSchedulerService(
        IBackgroundJobClient backgroundJobClient,
        IRecurringJobManager recurringJobManager)
    {
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
    }

    public string ScheduleCommand<TCommand>(TCommand command, TimeSpan delay) where TCommand : class, ICommand
    {
        return _backgroundJobClient.Schedule<JobService>(
            service => service.ExecuteCommandAsync(command, CancellationToken.None),
            delay);
    }

    public string EnqueueCommand<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        return _backgroundJobClient.Enqueue<JobService>(
            service => service.ExecuteCommandAsync(command, CancellationToken.None));
    }

    public string ScheduleRecurringCommand<TCommand>(string jobId, TCommand command, string cronExpression) where TCommand : class, ICommand
    {
        _recurringJobManager.AddOrUpdate<JobService>(
            jobId,
            service => service.ExecuteCommandAsync(command, CancellationToken.None),
            cronExpression);

        return jobId;
    }

    public void RemoveRecurringJob(string jobId)
    {
        _recurringJobManager.RemoveIfExists(jobId);
    }
}
