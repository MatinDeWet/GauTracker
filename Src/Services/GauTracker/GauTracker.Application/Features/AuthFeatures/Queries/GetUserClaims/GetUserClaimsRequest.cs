using System.Security.Claims;
using CQRS.Base;

namespace GauTracker.Application.Features.AuthFeatures.Queries.GetUserClaims;
public sealed record GetUserClaimsRequest(Guid UserId) : IQuery<List<Claim>>;
