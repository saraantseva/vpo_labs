using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Application.BlogerUseCases.Commands
{
    public sealed record DeleteBlogerCommand(int Id) : IRequest;

    internal class DeleteBlogerHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteBlogerCommand>
    {
        public async Task Handle(DeleteBlogerCommand request, CancellationToken cancellationToken)
        {
            var bloger = await unitOfWork.BlogerRepository.GetByIdAsync(request.Id, cancellationToken);

            if (bloger != null)
            {
                await unitOfWork.BlogerRepository.DeleteAsync(bloger, cancellationToken);
                await unitOfWork.SaveAllAsync();
            }
        }
    }
}
