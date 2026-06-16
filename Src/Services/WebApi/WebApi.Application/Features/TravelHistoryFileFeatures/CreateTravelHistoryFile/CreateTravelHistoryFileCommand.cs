using CQRS.Core.Contracts;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.CreateTravelHistoryFile;

public sealed record CreateTravelHistoryFileCommand(
    long CardId,
    string FileName,
    string? ContentType,
    string? DisplayName) : ICommand<CreateTravelHistoryFileResponse>;
