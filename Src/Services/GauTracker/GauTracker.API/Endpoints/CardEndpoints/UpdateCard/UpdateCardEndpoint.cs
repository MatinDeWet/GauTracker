using Ardalis.Result;
using CQRS.Core.Contracts;
using FastEndpoints;
using GauTracker.API.Common.Extensions;
using GauTracker.Application.Features.CardFeatures.Commands.UpdateCard;

namespace GauTracker.API.Endpoints.CardEndpoints.UpdateCard;

public class UpdateCardEndpoint(ICommandManager<UpdateCardRequest> manager) : Endpoint<UpdateCardRequest>
{
    public override void Configure()
    {
        Put("card/{Id}");
        Summary(s =>
        {
            s.Summary = "Update a card";
            s.Description = "Updates an existing card with the provided details";
        });
    }

    public override async Task HandleAsync(UpdateCardRequest req, CancellationToken ct)
    {
        Result result = await manager.Handle(req, ct);

        await this.SendResponse(result);
    }
}
