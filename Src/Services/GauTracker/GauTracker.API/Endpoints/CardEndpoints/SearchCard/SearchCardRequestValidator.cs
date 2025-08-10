using FastEndpoints;
using GauTracker.API.Common.Validation;
using GauTracker.Application.Features.CardFeatures.Queries.SearchCard;

namespace GauTracker.API.Endpoints.CardEndpoints.SearchCard;

public class SearchCardRequestValidator : Validator<SearchCardRequest>
{
    public SearchCardRequestValidator()
    {
        this.ValidatePageableRequest();
    }
}
