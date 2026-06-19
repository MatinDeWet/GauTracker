using Ardalis.Result;
using CQRS.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.CommandRepos.SecuredRepos;
using WebApi.Application.Repositories.QueryRepos.SecuredRepos;
using Shared.Domain.Entities;

namespace WebApi.Application.Features.CardFeatures.UpdateCard;

internal sealed class UpdateCardCommandHandler(
    ISecuredQueryRepo queryRepo,
    ISecuredCommandRepo commandRepo) : ICommandManager<UpdateCardCommand>
{
    public async Task<Result> Handle(UpdateCardCommand request, CancellationToken cancellationToken)
    {
        Card? card = await queryRepo.Cards
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (card is null)
        {
            return Result.NotFound();
        }

        card.Update(request.Name, request.Number, request.ExpiryDate);

        await commandRepo.UpdateAsync(card, persistImmediately: true, cancellationToken);

        return Result.Success();
    }
}
