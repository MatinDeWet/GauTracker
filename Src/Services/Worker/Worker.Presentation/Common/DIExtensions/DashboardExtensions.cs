using Hangfire;
using Worker.Presentation.Common.Dashboard;

namespace Worker.Presentation.Common.DIExtensions;

public static class DashboardExtensions
{
    /// <summary>
    /// Maps the Hangfire dashboard at <c>/hangfire</c>. In Development/Staging it uses a permissive
    /// authorization filter so the dashboard is reachable through Docker; otherwise Hangfire's
    /// default local-requests-only filter applies.
    /// </summary>
    public static WebApplication UseWorkerDashboard(this WebApplication app)
    {
        var options = new DashboardOptions();

        if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
        {
            options.Authorization = [new AllowAllDashboardAuthorizationFilter()];
        }

        app.UseHangfireDashboard("/hangfire", options);

        return app;
    }
}
