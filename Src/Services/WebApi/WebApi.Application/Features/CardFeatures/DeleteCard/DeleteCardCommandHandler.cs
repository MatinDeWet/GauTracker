using Ardalis.Result;
using CQRS.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.CommandRepos.SecuredRepos;
using WebApi.Application.Repositories.QueryRepos.SecuredRepos;
using Shared.Domain.Entities;

namespace WebApi.Application.Features.CardFeatures.DeleteCard;

internal sealed class DeleteCardCommandHandler(
    ISecuredQueryRepo queryRepo,
    ISecuredCommandRepo commandRepo) : ICommandManager<DeleteCardCommand>
{
    public async Task<Result> Handle(DeleteCardCommand request, CancellationToken cancellationToken)
    {
        Card? card = await queryRepo.Cards
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (card is null)
        {
            return Result.NotFound();
        }

        await commandRepo.DeleteAsync(card, persistImmediately: true, cancellationToken);

        return Result.Success();
    }
}
