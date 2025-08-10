using System.Text.Json.Serialization;

namespace Gautrain.Integration.Api.Models;

public class RouteResponse
{
    [JsonPropertyName("routes")]
    public List<Route> Routes { get; set; } = [];

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

public class Route
{
    [JsonPropertyName("legs")]
    public List<RouteLeg> Legs { get; set; } = [];

    [JsonPropertyName("overview_polyline")]
    public OverviewPolyline OverviewPolyline { get; set; } = new();

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;
}

public class RouteLeg
{
    [JsonPropertyName("distance")]
    public Distance Distance { get; set; } = new();

    [JsonPropertyName("duration")]
    public Duration Duration { get; set; } = new();

    [JsonPropertyName("end_address")]
    public string EndAddress { get; set; } = string.Empty;

    [JsonPropertyName("end_location")]
    public Location EndLocation { get; set; } = new();

    [JsonPropertyName("start_address")]
    public string StartAddress { get; set; } = string.Empty;

    [JsonPropertyName("start_location")]
    public Location StartLocation { get; set; } = new();

    [JsonPropertyName("steps")]
    public List<RouteStep> Steps { get; set; } = [];
}

public class RouteStep
{
    [JsonPropertyName("distance")]
    public Distance Distance { get; set; } = new();

    [JsonPropertyName("duration")]
    public Duration Duration { get; set; } = new();

    [JsonPropertyName("end_location")]
    public Location EndLocation { get; set; } = new();

    [JsonPropertyName("html_instructions")]
    public string HtmlInstructions { get; set; } = string.Empty;

    [JsonPropertyName("polyline")]
    public OverviewPolyline Polyline { get; set; } = new();

    [JsonPropertyName("start_location")]
    public Location StartLocation { get; set; } = new();

    [JsonPropertyName("travel_mode")]
    public string TravelMode { get; set; } = string.Empty;
}

public class Distance
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public int Value { get; set; }
}

public class Duration
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public int Value { get; set; }
}

public class Location
{
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    [JsonPropertyName("lng")]
    public double Longitude { get; set; }
}

public class OverviewPolyline
{
    [JsonPropertyName("points")]
    public string Points { get; set; } = string.Empty;
}
