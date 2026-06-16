using CQRS.Core.Contracts;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.DeleteTravelHistoryFile;

public sealed record DeleteTravelHistoryFileCommand(long Id) : ICommand;
