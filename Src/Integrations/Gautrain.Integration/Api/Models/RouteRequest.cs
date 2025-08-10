using System.Text.Json.Serialization;

namespace Gautrain.Integration.Api.Models;

public class RouteRequest
{
    [JsonPropertyName("orgLng")]
    public double OriginLongitude { get; set; }

    [JsonPropertyName("orgLat")]
    public double OriginLatitude { get; set; }

    [JsonPropertyName("dstLng")]
    public double DestinationLongitude { get; set; }

    [JsonPropertyName("dstLat")]
    public double DestinationLatitude { get; set; }

    [JsonPropertyName("publicOperators")]
    public string PublicOperators { get; set; } = string.Empty;

    [JsonPropertyName("isParking")]
    public bool IsParking { get; set; } = false;

    [JsonPropertyName("earliestArrival")]
    public string EarliestArrival { get; set; } = string.Empty;

    [JsonPropertyName("isGeometryReturned")]
    public bool IsGeometryReturned { get; set; } = true;

    [JsonPropertyName("isImmutable")]
    public bool IsImmutable { get; set; } = false;
}
