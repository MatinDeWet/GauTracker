using Domain.Support.Enums;

namespace Domain.Support.Contracts;
public interface IJobStatusAggregate
{
    JobStatusEnum Status { get; }

    void Transition(JobStatusEnum next, DateTimeOffset? now = null);

    void SetHangfireJobId(string jobId);

    void ClearError();

    void SetError(string? error);

    void IncrementRetry();
}
