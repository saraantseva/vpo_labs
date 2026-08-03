using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Application.BlogerUseCases.Queries
{   
    /// <summary>
    /// получить список всех блогеров
    /// </summary>
    public sealed record GetAllBlogersRequest() : IRequest<IEnumerable<Bloger>>;
    
    internal class GetAllBlogersHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllBlogersRequest, IEnumerable<Bloger>>
    {
        public async Task<IEnumerable<Bloger>> Handle(GetAllBlogersRequest request, CancellationToken cancel)
        {
            return await unitOfWork.BlogerRepository.ListAllAsync(cancel);
        }
    }
}
