using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Application.StoryUseCases.Queries
{
    /// <summary>
    /// получить список историй одного блогера(по id)
    /// </summary>
    public sealed record GetStoriesByBlogerIdRequest(int Id) : IRequest<IEnumerable<Story>>;

    public class GetStoriesByBlogerIdHandle(IUnitOfWork unitOfWork) : IRequestHandler<GetStoriesByBlogerIdRequest, IEnumerable<Story>> {
        
        public async Task<IEnumerable<Story>> Handle(GetStoriesByBlogerIdRequest request, CancellationToken token){
            return await unitOfWork.StoryRepository.ListAsync(s => s.BlogerId == request.Id, token);
        }
    
    }


}
