using FastEndpoints;
using FluentValidation;
using GauTracker.Application.Features.CardFeatures.Commands.DeleteCard;

namespace GauTracker.API.Endpoints.CardEndpoints.DeleteCard;

public class DeleteCardRequestValidator : Validator<DeleteCardRequest>
{
    public DeleteCardRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
