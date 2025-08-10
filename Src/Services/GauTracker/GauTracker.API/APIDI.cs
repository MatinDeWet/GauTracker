using GauTracker.Application.Common.Constants;
using GauTracker.Domain.Entities;
using GauTracker.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Identity;

namespace GauTracker.API;

public static class APIDI
{
    public static IServiceCollection AddIdentityPrepration(this IServiceCollection services)
    {
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(AuthConstants.LoginProvider)
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<GauTrackerContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
