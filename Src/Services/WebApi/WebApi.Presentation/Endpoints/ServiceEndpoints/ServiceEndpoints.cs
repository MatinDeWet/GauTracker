namespace WebApi.Presentation.Endpoints.ServiceEndpoints;

/// <summary>
/// Registers the <c>/services</c> endpoint group and maps its child endpoints.
/// </summary>
public static class ServiceEndpoints
{
    public static IEndpointRouteBuilder MapServiceEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/services").WithTags("Services");

        group.MapListServicesEndpoint();
        group.MapGetServiceByIdEndpoint();

        return app;
    }
}
