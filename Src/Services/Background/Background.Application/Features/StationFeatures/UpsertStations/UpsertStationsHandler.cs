using Ardalis.Result;
using Background.Application.Repositories.Command;
using Background.Application.Repositories.Query;
using CQRS.Core.Contracts;
using Domain.Core.Entities;
using Gautrain.Integration.Api;
using Gautrain.Integration.Api.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Background.Application.Features.StationFeatures.UpsertStations;

internal sealed class UpsertStationsHandler(
    IGautrainApi gautrainApi,
    IStationQueryRepository stationQueryRepository,
    IStationCommandRepository stationCommandRepository) : ICommandManager<UpsertStationsRequest>
{
    public async Task<Result> Handle(UpsertStationsRequest command, CancellationToken cancellationToken)
    {
        StationDataContext dataContext = await LoadDataContext(cancellationToken);

        await PreCreateNewTransportModes(dataContext.ApiStations, dataContext.ExistingTransportModes, cancellationToken);

        foreach (StationResponse apiStation in dataContext.ApiStations)
        {
            await ProcessStation(
                apiStation,
                dataContext.ExistingStations,
                dataContext.ExistingTransportModes,
                cancellationToken);
        }

        await stationCommandRepository.SaveAsync(cancellationToken);

        return Result.Success();
    }

    #region Data Loading

    private async Task<StationDataContext> LoadDataContext(CancellationToken cancellationToken)
    {
        Task<List<StationResponse>> apiStationsTask = gautrainApi.GetStationsAsync(cancellationToken);

        List<Station> existingStations = await stationQueryRepository.Stations
            .Include(s => s.StationTransportModes)
            .ThenInclude(stm => stm.TransportMode)
            .ToListAsync(cancellationToken);

        List<TransportMode> existingTransportModes = await stationQueryRepository.TransportModes
            .ToListAsync(cancellationToken);

        List<StationResponse> apiStations = await apiStationsTask;

        return new StationDataContext(
            apiStations,
            existingStations,
            existingTransportModes);
    }

    #endregion

    #region Station Processing

    private async Task ProcessStation(
        StationResponse apiStation,
        List<Station> existingStations,
        List<TransportMode> existingTransportModes,
        CancellationToken cancellationToken)
    {
        Point geometry = CreatePointFromCoordinates(apiStation.Geometry.Longitude, apiStation.Geometry.Latitude);
        Station? existingStation = FindExistingStation(apiStation.Id, existingStations);

        if (existingStation is null)
        {
            await CreateNewStation(apiStation, geometry, existingTransportModes, cancellationToken);
        }
        else
        {
            await UpdateExistingStation(existingStation, apiStation, geometry, existingTransportModes, cancellationToken);
        }
    }

    private async Task CreateNewStation(
        StationResponse apiStation,
        Point geometry,
        List<TransportMode> existingTransportModes,
        CancellationToken cancellationToken)
    {
        var newStation = Station.Create(apiStation.Id, apiStation.Name, geometry);

        foreach (string modeName in apiStation.Modes)
        {
            TransportMode transportMode = GetTransportMode(modeName, existingTransportModes);
            newStation.AddTransportMode(transportMode.Id);
        }

        await stationCommandRepository.InsertAsync(newStation, cancellationToken);
    }

    private async Task UpdateExistingStation(
        Station existingStation,
        StationResponse apiStation,
        Point geometry,
        List<TransportMode> existingTransportModes,
        CancellationToken cancellationToken)
    {
        await UpdateStationProperties(existingStation, apiStation, geometry, cancellationToken);
        await UpdateStationTransportModes(existingStation, apiStation.Modes, existingTransportModes, cancellationToken);
    }

    private async Task UpdateStationProperties(
        Station existingStation,
        StationResponse apiStation,
        Point geometry,
        CancellationToken cancellationToken)
    {
        bool stationUpdated = false;

        if (!existingStation.Name.Equals(apiStation.Name, StringComparison.Ordinal))
        {
            existingStation.UpdateName(apiStation.Name);
            stationUpdated = true;
        }

        if (!AreGeometriesEqual(existingStation.Location, geometry))
        {
            existingStation.UpdateGeometry(geometry);
            stationUpdated = true;
        }

        if (stationUpdated)
        {
            await stationCommandRepository.UpdateAsync(existingStation, cancellationToken);
        }
    }

    private async Task UpdateStationTransportModes(
        Station existingStation,
        List<string> apiModes,
        List<TransportMode> existingTransportModes,
        CancellationToken cancellationToken)
    {
        var currentTransportModeIds = existingStation.StationTransportModes
            .Select(stm => stm.TransportModeId)
            .ToHashSet();

        var transportModeIdsToAdd = new List<int>();
        foreach (string modeName in apiModes)
        {
            TransportMode transportMode = GetTransportMode(modeName, existingTransportModes);
            if (!currentTransportModeIds.Contains(transportMode.Id))
            {
                transportModeIdsToAdd.Add(transportMode.Id);
            }
        }

        var apiTransportModeIds = new HashSet<int>();
        foreach (string modeName in apiModes)
        {
            TransportMode transportMode = GetTransportMode(modeName, existingTransportModes);
            apiTransportModeIds.Add(transportMode.Id);
        }

        var transportModeIdsToRemove = currentTransportModeIds
            .Where(currentId => !apiTransportModeIds.Contains(currentId))
            .ToList();

        foreach (int transportModeId in transportModeIdsToAdd)
        {
            existingStation.AddTransportMode(transportModeId);
        }

        foreach (int transportModeId in transportModeIdsToRemove)
        {
            existingStation.RemoveTransportMode(transportModeId);
        }

        if (transportModeIdsToAdd.Any() || transportModeIdsToRemove.Any())
        {
            await stationCommandRepository.UpdateAsync(existingStation, cancellationToken);
        }
    }

    private static Station? FindExistingStation(string externalId, List<Station> existingStations)
    {
        return existingStations.FirstOrDefault(s =>
            s.ExternalId.Equals(externalId, StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region Transport Mode Management

    private async Task PreCreateNewTransportModes(
        List<StationResponse> apiStations,
        List<TransportMode> existingTransportModes,
        CancellationToken cancellationToken)
    {
        var allApiModeNames = apiStations
            .SelectMany(station => station.Modes)
            .Select(mode => mode.ToUpperInvariant())
            .ToHashSet();

        var existingModeNames = existingTransportModes
            .Select(tm => tm.Name.ToUpperInvariant())
            .ToHashSet();

        var newModeNames = allApiModeNames
            .Where(apiMode => !existingModeNames.Contains(apiMode))
            .ToList();

        foreach (string modeName in newModeNames)
        {
            var newTransportMode = TransportMode.Create(modeName);
            await stationCommandRepository.InsertAsync(newTransportMode, cancellationToken);
            existingTransportModes.Add(newTransportMode);
        }

        if (newModeNames.Any())
        {
            await stationCommandRepository.SaveAsync(cancellationToken);
        }
    }

    private static TransportMode GetTransportMode(string modeName, List<TransportMode> existingTransportModes)
    {
        TransportMode? transportMode = existingTransportModes.FirstOrDefault(tm =>
            tm.Name.Equals(modeName, StringComparison.OrdinalIgnoreCase));

        return transportMode ?? throw new InvalidOperationException($"Transport mode '{modeName}' should have been pre-created but was not found.");
    }

    #endregion

    #region Geometry Utilities

    private static Point CreatePointFromCoordinates(double longitude, double latitude)
    {
        var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        return geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
    }

    private static bool AreGeometriesEqual(Point? existing, Point? updated)
    {
        if (existing is null && updated is null)
        {
            return true;
        }

        if (existing is null || updated is null)
        {
            return false;
        }

        const double tolerance = 0.000001;

        return Math.Abs(existing.X - updated.X) < tolerance &&
               Math.Abs(existing.Y - updated.Y) < tolerance;
    }

    #endregion
}

internal sealed class StationDataContext(
    List<StationResponse> apiStations,
    List<Station> existingStations,
    List<TransportMode> existingTransportModes)
{
    public List<StationResponse> ApiStations { get; } = apiStations;
    public List<Station> ExistingStations { get; } = existingStations;
    public List<TransportMode> ExistingTransportModes { get; } = existingTransportModes;
}
