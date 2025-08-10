using System.Text.Json.Serialization;

namespace Gautrain.Integration.Api.Models;

public class TransitCardResponse
{
    [JsonPropertyName("cards")]
    public List<TransitCard> Cards { get; set; } = [];
}

public class TransitCard
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    [JsonPropertyName("alias")]
    public string Alias { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("balance")]
    public decimal Balance { get; set; }

    [JsonPropertyName("expiryDate")]
    public string ExpiryDate { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
