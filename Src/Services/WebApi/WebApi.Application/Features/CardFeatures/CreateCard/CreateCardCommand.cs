using CQRS.Core.Contracts;

namespace WebApi.Application.Features.CardFeatures.CreateCard;

public sealed record CreateCardCommand(string Name, string Number, DateOnly ExpiryDate) : ICommand<CreateCardResponse>;
