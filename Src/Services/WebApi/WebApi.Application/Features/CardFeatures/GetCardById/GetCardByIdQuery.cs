using CQRS.Core.Contracts;

namespace WebApi.Application.Features.CardFeatures.GetCardById;

public sealed record GetCardByIdQuery(long Id) : IQuery<GetCardByIdResponse>;
