using CQRS.Core.Contracts;

namespace WebApi.Application.Features.CardFeatures.UpdateCard;

public sealed record UpdateCardCommand(long Id, string Name, string Number, DateOnly ExpiryDate) : ICommand;
