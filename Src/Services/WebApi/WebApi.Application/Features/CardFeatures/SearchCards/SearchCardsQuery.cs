using CQRS.Core.Contracts;
using Pagination.Models.Requests;
using Pagination.Models.Responses;

namespace WebApi.Application.Features.CardFeatures.SearchCards;

public sealed class SearchCardsQuery : PageableRequest, IQuery<PageableResponse<SearchCardsResponse>>;
