using CQRS.Event.Base.Implementation;
using Domain.Core.Enums;
using Domain.Core.Events;
using Domain.Support.Contracts;
using Domain.Support.Enums;
using Domain.Support.Extensions;

namespace Domain.Core.Entities;
public class TransactionHistoryImportBatch : EventableEntity<Guid>, IJobStatusAggregate
{
    public Guid CardId { get; private set; }
    public virtual Card Card { get; private set; }

    // Blob info
    public string BlobContainer { get; private set; }

    public string BlobName { get; private set; }

    public string Sha256 { get; private set; }


    // Status & telemetry
    public JobStatusEnum Status { get; private set; } = JobStatusEnum.Uploaded;

    public TransactionImportOutcomeFlags OutcomeFlags { get; private set; } = TransactionImportOutcomeFlags.None;

    public int RetryCount { get; private set; }

    public string? Error { get; private set; }

    // Timestamps (audit)
    public DateTimeOffset UploadedAt { get; private set; }

    public DateTimeOffset? QueuedAt { get; private set; }

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public DateTimeOffset? FailedAt { get; private set; }

    public DateTimeOffset? CanceledAt { get; private set; }

    public DateTimeOffset? SupersededAt { get; private set; }

    // Concurrency / background processing
    public string? HangfireJobId { get; private set; }

    public uint Version { get; private set; }

    public static TransactionHistoryImportBatch Create(Guid cardId, string container, string name, string sha256)
    {
        if (cardId == Guid.Empty)
        {
            throw new ArgumentException("Card ID cannot be empty.", nameof(cardId));
        }

        ArgumentNullException.ThrowIfNullOrWhiteSpace(container, nameof(container));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(sha256, nameof(sha256));

        var transactionHistoryImportBatch = new TransactionHistoryImportBatch
        {
            Id = Guid.CreateVersion7(),
            CardId = cardId,
            BlobContainer = container,
            BlobName = name,
            Sha256 = sha256,
            UploadedAt = DateTimeOffset.UtcNow,
            Status = JobStatusEnum.Uploaded
        };

        transactionHistoryImportBatch.AddDomainEvent(new TransactionHistoryImportBatchCreatedEvent(transactionHistoryImportBatch));

        return transactionHistoryImportBatch;
    }

    public void Transition(JobStatusEnum next, DateTimeOffset? now = null)
    {
        if (!JobStatusPolicy.CanTransition(Status, next))
        {
            throw new InvalidOperationException($"Illegal transition {Status} -> {next}");
        }

        Status = next;
        Stamp(next, now ?? DateTimeOffset.UtcNow);
    }

    public void SetHangfireJobId(string jobId)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(jobId, nameof(jobId));

        if (!string.IsNullOrEmpty(HangfireJobId) && HangfireJobId != jobId)
        {
            throw new InvalidOperationException($"Already queued with job id {HangfireJobId}");
        }

        HangfireJobId ??= jobId;
    }

    public void AddOutcomeFlags(TransactionImportOutcomeFlags flags) => OutcomeFlags |= flags;

    public void ClearError() => Error = null;

    public void SetError(string? error) => Error = Truncate(error, 2000);

    public void IncrementRetry() => RetryCount++;

    public bool IsTerminal => JobStatusPolicy.IsTerminal(Status);

    public IReadOnlyList<JobStatusEnum> AllowedNext() => JobStatusPolicy.AllowedNext(Status);

    private void Stamp(JobStatusEnum next, DateTimeOffset ts)
    {
        switch (next)
        {
            case JobStatusEnum.Queued:
                QueuedAt ??= ts;
                break;
            case JobStatusEnum.Processing:
                StartedAt ??= ts;
                break;
            case JobStatusEnum.Completed:
                CompletedAt ??= ts;
                break;
            case JobStatusEnum.Failed:
                FailedAt ??= ts;
                break;
            case JobStatusEnum.Canceled:
                CanceledAt ??= ts;
                break;
            case JobStatusEnum.Superseded:
                SupersededAt ??= ts;
                break;
            case JobStatusEnum.Duplicate:
                break;
        }
    }

    private static string? Truncate(string? value, int max) =>
        string.IsNullOrEmpty(value) ? value : (value!.Length <= max ? value : value[..max]);
}
