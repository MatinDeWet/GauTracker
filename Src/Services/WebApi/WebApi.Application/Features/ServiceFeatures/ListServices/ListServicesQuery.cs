using CQRS.Core.Contracts;

namespace WebApi.Application.Features.ServiceFeatures.ListServices;

public sealed record ListServicesQuery : IQuery<IReadOnlyList<ListServicesResponse>>;
