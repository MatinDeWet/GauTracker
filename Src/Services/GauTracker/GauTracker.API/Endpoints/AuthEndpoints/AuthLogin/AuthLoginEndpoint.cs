using System.Security.Claims;
using Ardalis.Result;
using CQRS.Core.Contracts;
using FastEndpoints;
using GauTracker.API.Common.Extensions;
using GauTracker.API.Endpoints.AuthEndpoints.Common;
using GauTracker.Application.Features.AuthFeatures.Queries.GetUserClaims;
using GauTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GauTracker.API.Endpoints.AuthEndpoints.AuthLogin;

public class AuthLoginEndpoint(UserManager<ApplicationUser> userManager, IQueryManager<GetUserClaimsRequest, List<Claim>> userClaimHandler) : Endpoint<AuthLoginRequest, ApplicationTokenResponse>
{
    public override void Configure()
    {
        Post("auth/login");
        AllowAnonymous();
        Summary(x =>
        {
            x.Summary = "Login to the application";
            x.Description = "This endpoint allows users to log in using their email and password.";
            x.Response<ApplicationTokenResponse>(StatusCodes.Status200OK, "Login successful");
            x.Response(StatusCodes.Status400BadRequest, "Invalid request");
            x.Response(StatusCodes.Status401Unauthorized, "Unauthorized - Invalid Email / Password");
        });
    }

    public override async Task HandleAsync(AuthLoginRequest req, CancellationToken ct)
    {
        ApplicationUser? user = await userManager.FindByEmailAsync(req.Email);

        if (user is null)
        {
            ThrowError("Invalid Email / Password", StatusCodes.Status401Unauthorized);
        }

        bool isValidPassword = await userManager.CheckPasswordAsync(user, req.Password);

        if (!isValidPassword)
        {
            ThrowError("Invalid Email / Password", StatusCodes.Status401Unauthorized);
        }

        Result<List<Claim>> result = await userClaimHandler.Handle(new GetUserClaimsRequest(user.Id), ct);

        await this.SendResponseAsync(result, async claims => await CreateTokenWith<UserTokenService>(user.Id.ToString(), options => options.Claims.AddRange(claims.Value)));
    }
}
