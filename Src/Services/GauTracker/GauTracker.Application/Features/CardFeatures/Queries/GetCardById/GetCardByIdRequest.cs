using CQRS.Base;

namespace GauTracker.Application.Features.CardFeatures.Queries.GetCardById;

public sealed record GetCardByIdRequest : IQuery<GetCardByIdResponse>
{
    public Guid Id { get; set; }
}
