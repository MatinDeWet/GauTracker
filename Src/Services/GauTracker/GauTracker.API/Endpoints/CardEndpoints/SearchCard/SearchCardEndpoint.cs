using Ardalis.Result;
using CQRS.Core.Contracts;
using FastEndpoints;
using GauTracker.API.Common.Extensions;
using GauTracker.Application.Features.CardFeatures.Queries.SearchCard;
using Pagination.Models.Responses;

namespace GauTracker.API.Endpoints.CardEndpoints.SearchCard;

public class SearchCardEndpoint(IQueryManager<SearchCardRequest, PageableResponse<SearchCardResponse>> manager) : Endpoint<SearchCardRequest, PageableResponse<SearchCardResponse>>
{
    public override void Configure()
    {
        Get("card");
        Summary(s =>
        {
            s.Summary = "Search cards";
            s.Description = "Retrieves a paginated list of cards with optional search filters";
        });
    }

    public override async Task HandleAsync(SearchCardRequest req, CancellationToken ct)
    {
        Result<PageableResponse<SearchCardResponse>> result = await manager.Handle(req, ct);

        await this.SendResponse(result, response => response.GetValue());
    }
}
