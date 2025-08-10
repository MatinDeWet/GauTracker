using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Gautrain.Integration.Api.Models;
using RestSharp;

namespace Gautrain.Integration.Api;

public class GautrainApi : IGautrainApi, IDisposable
{
    private readonly RestClient _client;
    private readonly JsonSerializerOptions _jsonOptions;
    private string? _csrfToken;
    private string? _sessionCookie;

    public GautrainApi()
    {
        RestClientOptions options = new("https://www.gautrain.co.za")
        {
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
        };

        _client = new RestClient(options);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<List<StationResponse>> GetStationsAsync(CancellationToken cancellationToken = default)
    {
        RestRequest request = new("/commuter/stations", Method.Get);

        RestResponse response = await _client.ExecuteAsync(request, cancellationToken);

        if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
        {
            throw new HttpRequestException($"Failed to get stations: {response.ErrorMessage}");
        }

        List<StationResponse>? stations = JsonSerializer.Deserialize<List<StationResponse>>(response.Content, _jsonOptions);
        return stations ?? [];
    }

    public async Task<RouteResponse> GetRouteAsync(RouteRequest routeRequest, CancellationToken cancellationToken = default)
    {
        RestRequest request = new("/commuter/route", Method.Get);

        request.AddQueryParameter("orgLng", routeRequest.OriginLongitude.ToString(CultureInfo.InvariantCulture));
        request.AddQueryParameter("orgLat", routeRequest.OriginLatitude.ToString(CultureInfo.InvariantCulture));
        request.AddQueryParameter("dstLng", routeRequest.DestinationLongitude.ToString(CultureInfo.InvariantCulture));
        request.AddQueryParameter("dstLat", routeRequest.DestinationLatitude.ToString(CultureInfo.InvariantCulture));
        request.AddQueryParameter("publicOperators", routeRequest.PublicOperators);
        request.AddQueryParameter("isParking", routeRequest.IsParking.ToString(CultureInfo.InvariantCulture).ToUpperInvariant());
        request.AddQueryParameter("earliestArrival", routeRequest.EarliestArrival);
        request.AddQueryParameter("isGeometryReturned", routeRequest.IsGeometryReturned.ToString(CultureInfo.InvariantCulture).ToUpperInvariant());
        request.AddQueryParameter("isImmutable", routeRequest.IsImmutable.ToString(CultureInfo.InvariantCulture).ToUpperInvariant());

        RestResponse response = await _client.ExecuteAsync(request, cancellationToken);

        if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
        {
            throw new HttpRequestException($"Failed to get route: {response.ErrorMessage}");
        }

        RouteResponse? routeResponse = JsonSerializer.Deserialize<RouteResponse>(response.Content, _jsonOptions);
        return routeResponse ?? new RouteResponse();
    }

    public async Task<bool> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
    {
        // Step 1: Get CSRF token from login page
        await GetCsrfTokenAsync(cancellationToken);

        if (string.IsNullOrEmpty(_csrfToken))
        {
            throw new InvalidOperationException("Failed to retrieve CSRF token");
        }

        // Step 2: Perform login with credentials and CSRF token
        RestRequest request = new("/login", Method.Post);
        request.AddParameter("username", loginRequest.Username, ParameterType.GetOrPost);
        request.AddParameter("password", loginRequest.Password, ParameterType.GetOrPost);
        request.AddParameter("_csrf", _csrfToken, ParameterType.GetOrPost);

        RestResponse response = await _client.ExecuteAsync(request, cancellationToken);

        if (response.IsSuccessful)
        {
            // Extract session cookie from response
            string? sessionCookie = response.Cookies?.FirstOrDefault(c => c.Name == "SESSION")?.Value;
            if (!string.IsNullOrEmpty(sessionCookie))
            {
                _sessionCookie = sessionCookie;
                return true;
            }
        }

        return false;
    }

    public async Task<TransitCardResponse> GetTransitCardsAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_sessionCookie))
        {
            throw new InvalidOperationException("Must login first before getting transit cards");
        }

        RestRequest request = new("/commuter/transitcards", Method.Get);
        request.AddHeader("Cookie", $"SESSION={_sessionCookie}");

        RestResponse response = await _client.ExecuteAsync(request, cancellationToken);

        if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
        {
            throw new HttpRequestException($"Failed to get transit cards: {response.ErrorMessage}");
        }

        TransitCardResponse? transitCards = JsonSerializer.Deserialize<TransitCardResponse>(response.Content, _jsonOptions);
        return transitCards ?? new TransitCardResponse();
    }

    private async Task GetCsrfTokenAsync(CancellationToken cancellationToken)
    {
        RestRequest request = new("/login", Method.Get);
        RestResponse response = await _client.ExecuteAsync(request, cancellationToken);

        if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
        {
            // Extract CSRF token from HTML response using regex
            Match match = Regex.Match(response.Content, @"<input[^>]*name=['""]_csrf['""][^>]*value=['""]([^'""]*)['""]", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                _csrfToken = match.Groups[1].Value;
            }
        }
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}
