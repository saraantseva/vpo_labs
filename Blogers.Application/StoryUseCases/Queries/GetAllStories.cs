using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Application.StoryUseCases.Queries
{
    public sealed record GetAllStoriesRequest() : IRequest<IEnumerable<Story>>;
    
    internal class GetAllStoriesHandler(IUnitOfWork UnitOfWork): IRequestHandler<GetAllStoriesRequest, IEnumerable<Story>>
    {
        public async Task<IEnumerable<Story>> Handle(GetAllStoriesRequest request, CancellationToken cancell)
        {
            return await UnitOfWork.StoryRepository.ListAllAsync(cancell);
        }
    }
}



