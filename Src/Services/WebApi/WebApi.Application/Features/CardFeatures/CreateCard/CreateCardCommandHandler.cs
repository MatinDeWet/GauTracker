using Ardalis.Result;
using CQRS.Core.Contracts;
using Identification.Contracts;
using WebApi.Application.Repositories.CommandRepos.SecuredRepos;
using Shared.Domain.Entities;

namespace WebApi.Application.Features.CardFeatures.CreateCard;

internal sealed class CreateCardCommandHandler(
    ISecuredCommandRepo commandRepo,
    IIdentityInfo identityInfo) : ICommandManager<CreateCardCommand, CreateCardResponse>
{
    public async Task<Result<CreateCardResponse>> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
        var card = Card.Create(identityInfo.GetInternalUserId(), request.Name, request.Number, request.ExpiryDate);

        await commandRepo.InsertAsync(card, persistImmediately: true, cancellationToken);

        return new CreateCardResponse(card.Id);
    }
}
