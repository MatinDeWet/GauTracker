using CQRS.Core.Contracts;

namespace WebApi.Application.Features.CardFeatures.DeleteCard;

public sealed record DeleteCardCommand(long Id) : ICommand;
