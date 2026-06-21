namespace WebApi.Presentation.Endpoints.StationEndpoints;

/// <summary>
/// Registers the <c>/stations</c> endpoint group and maps its child endpoints.
/// </summary>
public static class StationEndpoints
{
    public static IEndpointRouteBuilder MapStationEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/stations").WithTags("Stations");

        group.MapListStationsEndpoint();
        group.MapGetStationByIdEndpoint();
        group.MapGetStationServicesEndpoint();

        return app;
    }
}
