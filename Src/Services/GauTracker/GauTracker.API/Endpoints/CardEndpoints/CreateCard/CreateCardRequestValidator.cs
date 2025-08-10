using FastEndpoints;
using FluentValidation;
using GauTracker.API.Common.Validation;
using GauTracker.Application.Features.CardFeatures.Commands.CreateCard;

namespace GauTracker.API.Endpoints.CardEndpoints.CreateCard;

public class CreateCardRequestValidator : Validator<CreateCardRequest>
{
    public CreateCardRequestValidator()
    {
        RuleFor(x => x.Alias)
            .StringInput(64);

        RuleFor(x => x.Number)
            .StringInput(128);

        RuleFor(x => x.ExpiryDate)
            .NotEmpty();
    }
}
