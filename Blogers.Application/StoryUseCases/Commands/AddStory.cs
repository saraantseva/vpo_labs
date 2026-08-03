using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Application.StoryUseCases.Commands
{
    public sealed record class AddStoryRequest(string description,
                    int? likeCount, int? viewCount, int blogerId) : IRequest<Story>;

    internal class AddStoryHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddStoryRequest, Story> { 
    
        public async Task<Story> Handle(AddStoryRequest request, CancellationToken cancellationToken)
        {
            var story = new Story(request.description, request.likeCount, request.viewCount, request.blogerId);

            await unitOfWork.StoryRepository.AddAsync(story);
            await unitOfWork.SaveAllAsync();
            return story;
        } 
    }


}
