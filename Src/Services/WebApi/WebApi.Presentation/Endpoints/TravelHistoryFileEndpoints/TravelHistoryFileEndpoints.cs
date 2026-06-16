namespace WebApi.Presentation.Endpoints.TravelHistoryFileEndpoints;

/// <summary>
/// Registers the travel-history-file endpoints. Collection operations are nested under the owning
/// card (<c>/cards/{cardId}/travel-history-files</c>); item operations are flat by file id
/// (<c>/travel-history-files/{id}</c>).
/// </summary>
public static class TravelHistoryFileEndpoints
{
    public static IEndpointRouteBuilder MapTravelHistoryFileEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder cardScoped = app
            .MapGroup("/cards/{cardId:long}/travel-history-files")
            .WithTags("Travel History Files");

        cardScoped.MapSearchTravelHistoryFilesEndpoint();
        cardScoped.MapCreateTravelHistoryFileEndpoint();

        RouteGroupBuilder files = app
            .MapGroup("/travel-history-files")
            .WithTags("Travel History Files");

        files.MapGetTravelHistoryFileByIdEndpoint();
        files.MapGetTravelHistoryFileUploadUrlEndpoint();
        files.MapConfirmTravelHistoryFileUploadEndpoint();
        files.MapGetTravelHistoryFileDownloadUrlEndpoint();
        files.MapUpdateTravelHistoryFileEndpoint();
        files.MapDeleteTravelHistoryFileEndpoint();

        return app;
    }
}
