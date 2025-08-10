using System.Text.Json.Serialization;

namespace Gautrain.Integration.Api.Models;

public class StationResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;

    [JsonPropertyName("agency")]
    public Agency Agency { get; set; } = new();

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("geometry")]
    public Geometry Geometry { get; set; } = new();

    [JsonPropertyName("modes")]
    public List<string> Modes { get; set; } = [];
}

public class Agency
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("culture")]
    public string Culture { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;
}

public class Geometry
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("coordinates")]
    public List<double> Coordinates { get; set; } = [];

    /// <summary>
    /// Gets the longitude (first coordinate)
    /// </summary>
    public double Longitude => Coordinates.Count > 0 ? Coordinates[0] : 0;

    /// <summary>
    /// Gets the latitude (second coordinate)
    /// </summary>
    public double Latitude => Coordinates.Count > 1 ? Coordinates[1] : 0;
}
