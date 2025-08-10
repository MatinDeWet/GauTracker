using System.Security.Claims;
using Ardalis.Result;
using CQRS.Core.Contracts;
using GauTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GauTracker.Application.Features.AuthFeatures.Queries.GetUserClaims;
internal sealed class GetUserClaimsHandler(UserManager<ApplicationUser> userManager) : IQueryManager<GetUserClaimsRequest, List<Claim>>
{
    public async Task<Result<List<Claim>>> Handle(GetUserClaimsRequest query, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(query.UserId.ToString());

        if (user is null)
        {
            return Result.NotFound();
        }

        IList<Claim> userClaims = await userManager.GetClaimsAsync(user);

        IList<string> roles = await userManager.GetRolesAsync(user);
        IList<Claim> roleClaims = [.. roles.Select(role => new Claim(ClaimTypes.Role, role))];


        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
        }
        .Union(roleClaims)
        .Union(userClaims)
        .ToList();

        return claims;
    }
}
