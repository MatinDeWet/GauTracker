using CQRS.Core.Contracts;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.ConfirmTravelHistoryFileUpload;

public sealed record ConfirmTravelHistoryFileUploadCommand(long Id) : ICommand;
