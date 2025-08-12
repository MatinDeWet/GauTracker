using System.Collections.Immutable;
using Domain.Support.Enums;

namespace Domain.Support.Extensions;
public static class JobStatusPolicy
{
    private static readonly ImmutableDictionary<JobStatusEnum, JobStatusEnum[]> Allowed =
        new Dictionary<JobStatusEnum, JobStatusEnum[]>
        {
            [JobStatusEnum.Uploaded] = [JobStatusEnum.Queued, JobStatusEnum.Canceled, JobStatusEnum.Duplicate],
            [JobStatusEnum.Queued] = [JobStatusEnum.Processing, JobStatusEnum.Canceled],
            [JobStatusEnum.Processing] = [JobStatusEnum.Completed, JobStatusEnum.Failed, JobStatusEnum.Canceled, JobStatusEnum.Superseded],
        }.ToImmutableDictionary();

    private static readonly HashSet<JobStatusEnum> Terminal = [JobStatusEnum.Completed, JobStatusEnum.Failed, JobStatusEnum.Canceled, JobStatusEnum.Duplicate, JobStatusEnum.Superseded];

    public static bool CanTransition(JobStatusEnum current, JobStatusEnum next) =>
        Allowed.TryGetValue(current, out JobStatusEnum[]? nexts) && nexts.Contains(next);

    public static bool IsTerminal(JobStatusEnum s) => Terminal.Contains(s);

    public static IReadOnlyList<JobStatusEnum> AllowedNext(JobStatusEnum current) =>
        Allowed.TryGetValue(current, out JobStatusEnum[]? nexts) ? nexts : [];
}
