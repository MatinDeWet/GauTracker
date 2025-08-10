using CQRS.Base;

namespace Background.Application.Services;
public interface IJobSchedulerService
{
    string ScheduleCommand<TCommand>(TCommand command, TimeSpan delay) where TCommand : class, ICommand;

    string EnqueueCommand<TCommand>(TCommand command) where TCommand : class, ICommand;

    string ScheduleRecurringCommand<TCommand>(string jobId, TCommand command, string cronExpression) where TCommand : class, ICommand;

    void RemoveRecurringJob(string jobId);
}
