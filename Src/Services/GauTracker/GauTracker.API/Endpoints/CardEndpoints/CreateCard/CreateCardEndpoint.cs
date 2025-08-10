using Ardalis.Result;
using CQRS.Core.Contracts;
using FastEndpoints;
using GauTracker.API.Common.Extensions;
using GauTracker.Application.Features.CardFeatures.Commands.CreateCard;

namespace GauTracker.API.Endpoints.CardEndpoints.CreateCard;

public class CreateCardEndpoint(ICommandManager<CreateCardRequest, Guid> manager) : Endpoint<CreateCardRequest, Guid>
{
    public override void Configure()
    {
        Post("card");
        Summary(s =>
        {
            s.Summary = "Create a new card";
            s.Description = "Creates a new card with the provided details";
        });
    }
    public override async Task HandleAsync(CreateCardRequest req, CancellationToken ct)
    {
        Result<Guid> result = await manager.Handle(req, ct);

        await this.SendResponse(result, response => response.GetValue());
    }
}
