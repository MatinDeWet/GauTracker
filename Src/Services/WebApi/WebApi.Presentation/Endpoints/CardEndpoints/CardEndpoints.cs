namespace WebApi.Presentation.Endpoints.CardEndpoints;

/// <summary>
/// Registers the <c>/cards</c> endpoint group and maps its child endpoints.
/// </summary>
public static class CardEndpoints
{
    public static IEndpointRouteBuilder MapCardEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/cards").WithTags("Cards");

        group.MapSearchCardsEndpoint();
        group.MapGetCardByIdEndpoint();
        group.MapCreateCardEndpoint();
        group.MapUpdateCardEndpoint();
        group.MapDeleteCardEndpoint();

        return app;
    }
}
