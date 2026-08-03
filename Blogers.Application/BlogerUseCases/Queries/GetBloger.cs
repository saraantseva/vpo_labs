using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Application.BlogerUseCases.Queries
{
    /// <summary>
    /// получить блогера(по id) информация
    /// </summary>
    public sealed record GetBlogerRequest(int Id) : IRequest<Bloger>;

    internal class GetBlogerHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetBlogerRequest, Bloger>
    {
        public async Task<Bloger> Handle(GetBlogerRequest request, CancellationToken cancel)
        {
            return await unitOfWork.BlogerRepository.GetByIdAsync(request.Id, cancel);
        }
    }
}
