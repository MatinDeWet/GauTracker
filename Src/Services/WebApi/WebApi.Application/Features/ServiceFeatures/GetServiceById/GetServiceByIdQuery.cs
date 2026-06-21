using CQRS.Core.Contracts;

namespace WebApi.Application.Features.ServiceFeatures.GetServiceById;

public sealed record GetServiceByIdQuery(int Id) : IQuery<GetServiceByIdResponse>;
