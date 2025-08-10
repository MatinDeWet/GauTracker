using FastEndpoints;
using FluentValidation;
using GauTracker.Application.Features.CardFeatures.Queries.GetCardById;

namespace GauTracker.API.Endpoints.CardEndpoints.GetCardById;

public class GetCardByIdRequestValidator : Validator<GetCardByIdRequest>
{
    public GetCardByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
