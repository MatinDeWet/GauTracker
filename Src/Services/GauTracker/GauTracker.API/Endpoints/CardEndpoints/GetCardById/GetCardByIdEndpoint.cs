using Ardalis.Result;
using CQRS.Core.Contracts;
using FastEndpoints;
using GauTracker.API.Common.Extensions;
using GauTracker.Application.Features.CardFeatures.Queries.GetCardById;

namespace GauTracker.API.Endpoints.CardEndpoints.GetCardById;

public class GetCardByIdEndpoint(IQueryManager<GetCardByIdRequest, GetCardByIdResponse> manager) : Endpoint<GetCardByIdRequest, GetCardByIdResponse>
{
    public override void Configure()
    {
        Get("card/{Id}");
        Summary(s =>
        {
            s.Summary = "Get a card by ID";
            s.Description = "Retrieves a specific card by its unique identifier";
        });
    }

    public override async Task HandleAsync(GetCardByIdRequest req, CancellationToken ct)
    {
        Result<GetCardByIdResponse> result = await manager.Handle(req, ct);

        await this.SendResponse(result, response => response.GetValue());
    }
}
