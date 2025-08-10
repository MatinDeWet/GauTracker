using System.Security.Claims;
using Ardalis.Result;
using CQRS.Core.Contracts;
using FastEndpoints;
using FastEndpoints.Security;
using GauTracker.Application.Features.AuthFeatures.Queries.GetUserClaims;
using GauTracker.Application.Repositories.Command;
using GauTracker.Application.Repositories.Query;

namespace GauTracker.API.Endpoints.AuthEndpoints.Common;

public class UserTokenService : RefreshTokenService<TokenRequest, ApplicationTokenResponse>
{
    private readonly IUserRefreshTokenCommandRepository _commandRepo;
    private readonly IUserRefreshTokenQueryRepository _queryRepo;
    private readonly IQueryManager<GetUserClaimsRequest, List<Claim>> _userClaimHandler;

    public UserTokenService(IConfiguration config, IUserRefreshTokenCommandRepository commandRepo, IUserRefreshTokenQueryRepository queryRepo, IQueryManager<GetUserClaimsRequest, List<Claim>> userClaimHandler)
    {
        _commandRepo = commandRepo;
        _queryRepo = queryRepo;
        _userClaimHandler = userClaimHandler;

        Setup(x =>
        {
            x.TokenSigningKey = config["Auth:JWTSigningKey"];
            x.AccessTokenValidity = TimeSpan.FromMinutes(config.GetValue<int>("Auth:AccessTokenValidityMinutes"));
            x.RefreshTokenValidity = TimeSpan.FromMinutes(config.GetValue<int>("Auth:RefreshTokenValidityMinutes"));
            x.Endpoint("auth/refresh-token", ep =>
            {
                ep.Summary(s =>
                {
                    s.Summary = "Refresh JWT Token";
                    s.Description = "Refreshes an expired JWT access token using a valid refresh token";
                });
                ep.Options(b => b.WithName("AuthRefreshToken"));
            });
        });
    }

    public override async Task PersistTokenAsync(ApplicationTokenResponse response)
        => await _commandRepo.StoreToken(new Guid(response.UserId), response.RefreshToken, response.RefreshExpiry);

    public override async Task RefreshRequestValidationAsync(TokenRequest req)
    {
        bool isValidToken = await _queryRepo.IsValidToken(new Guid(req.UserId), req.RefreshToken);

        if (!isValidToken)
        {
            AddError("The refresh token is not valid!");
        }
    }

    public override async Task SetRenewalPrivilegesAsync(TokenRequest request, UserPrivileges privileges)
    {
        Result<List<Claim>> result = await _userClaimHandler.Handle(new GetUserClaimsRequest(new Guid(request.UserId)), CancellationToken.None);

        if (result.IsSuccess)
        {
            privileges.Claims.AddRange(result.Value);
        }
    }
}
