using Hangfire.Dashboard;

namespace Worker.Presentation.Common.Dashboard;

/// <summary>
/// Permissive dashboard filter used in Development/Staging so the dashboard is reachable through
/// Docker (the default filter allows local requests only). Do not use in Production — gate the
/// dashboard behind real authentication there.
/// </summary>
internal sealed class AllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}
