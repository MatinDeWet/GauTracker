using System.Security.Claims;
using Identification.Base;

namespace GauTracker.API.Common.Filters;

public sealed class IdentityInfoFilter(IInfoSetter setter) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        ClaimsPrincipal user = context.HttpContext.User;

        if (user.Identity is not null && user.Identity.IsAuthenticated)
        {
            var claims = user.Claims.ToList();
            setter.SetUser(claims);
        }

        return await next(context);
    }
}
