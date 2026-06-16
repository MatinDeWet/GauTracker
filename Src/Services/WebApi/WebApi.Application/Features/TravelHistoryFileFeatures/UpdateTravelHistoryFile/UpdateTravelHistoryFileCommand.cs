using CQRS.Core.Contracts;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.UpdateTravelHistoryFile;

public sealed record UpdateTravelHistoryFileCommand(long Id, string FileName, string? DisplayName) : ICommand;
