using CQRS.Base;
using Pagination.Models.Requests;
using Pagination.Models.Responses;

namespace GauTracker.Application.Features.CardFeatures.Queries.SearchCard;
public sealed class SearchCardRequest : PageableRequest, IQuery<PageableResponse<SearchCardResponse>>
{
}
