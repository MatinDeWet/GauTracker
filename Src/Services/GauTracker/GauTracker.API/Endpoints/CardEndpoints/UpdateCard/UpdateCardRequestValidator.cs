using FastEndpoints;
using FluentValidation;
using GauTracker.API.Common.Validation;
using GauTracker.Application.Features.CardFeatures.Commands.UpdateCard;

namespace GauTracker.API.Endpoints.CardEndpoints.UpdateCard;

public class UpdateCardRequestValidator : Validator<UpdateCardRequest>
{
    public UpdateCardRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Alias)
            .StringInput(64);

        RuleFor(x => x.Number)
            .StringInput(128);

        RuleFor(x => x.ExpiryDate)
            .NotEmpty();
    }
}
