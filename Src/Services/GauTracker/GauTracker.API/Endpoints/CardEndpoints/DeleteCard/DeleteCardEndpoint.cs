using Ardalis.Result;
using CQRS.Core.Contracts;
using FastEndpoints;
using GauTracker.API.Common.Extensions;
using GauTracker.Application.Features.CardFeatures.Commands.DeleteCard;

namespace GauTracker.API.Endpoints.CardEndpoints.DeleteCard;

public class DeleteCardEndpoint(ICommandManager<DeleteCardRequest> manager) : Endpoint<DeleteCardRequest>
{
    public override void Configure()
    {
        Delete("card/{Id}");
        Summary(s =>
        {
            s.Summary = "Delete a card";
            s.Description = "Deletes a card by its ID";
        });
    }

    public override async Task HandleAsync(DeleteCardRequest req, CancellationToken ct)
    {
        Result result = await manager.Handle(req, ct);

        await this.SendResponse(result);
    }
}
