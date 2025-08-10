using FastEndpoints;
using GauTracker.Application.Repositories.Command;
using GauTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using configuredUser = Domain.Core.Entities.User;

namespace GauTracker.API.Endpoints.AuthEndpoints.AuthRegister;

public class AuthRegisterEndpoint(UserManager<ApplicationUser> userManager, IUserCommandRepository repo) : Endpoint<AuthRegisterRequest>
{
    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
        Summary(x =>
        {
            x.Summary = "Register a new user";
            x.Description = "This endpoint allows users to register with an email and password.";
        });
    }

    public override async Task HandleAsync(AuthRegisterRequest request, CancellationToken cancellationToken)
    {
        var applicationUser = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
        };

        IdentityResult? result = await userManager.CreateAsync(applicationUser, request.Password);

        if (result is null)
        {
            ThrowError("User creation failed!");
        }

        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                AddError(error.Description);
            }

            ThrowIfAnyErrors();
        }

        ApplicationUser? identityUser = await userManager.FindByEmailAsync(request.Email);

        ArgumentNullException.ThrowIfNull(identityUser, nameof(identityUser));

        var user = configuredUser.Create(identityUser.Id, request.Email);

        await repo.InsertAsync(user, true, cancellationToken);

        ThrowIfAnyErrors();
    }
}
