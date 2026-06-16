namespace WebApi.Application.Features.CardFeatures.GetCardById;

public sealed record GetCardByIdResponse(long Id, string Name, string Number, DateOnly ExpiryDate, DateTimeOffset DateCreated);
