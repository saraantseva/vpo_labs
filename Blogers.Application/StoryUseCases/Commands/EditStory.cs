using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Application.StoryUseCases.Commands
{
    public sealed record class EditStoryRequest(int Id, string description,
                int? likeCount, int? viewCount, int blogerId) : IRequest<Story>;

    internal class EditStoryHandler(IUnitOfWork unitOfWork) : IRequestHandler<EditStoryRequest, Story>
    {

        public async Task<Story> Handle(EditStoryRequest request, CancellationToken cancellationToken)
        {
            Story story = await unitOfWork.StoryRepository.GetByIdAsync(request.Id);
            if (story == null)
            {
                return null;
            }

            story.Changedescription(request.description);
            story.ChangeViewCount(request.viewCount ?? 0);
            story.ChangeLikesCount(request.likeCount ?? 0);

            story.AssociateWith(request.blogerId);


            await unitOfWork.StoryRepository.UpdateAsync(story, cancellationToken);
            await unitOfWork.SaveAllAsync();
            return story;

        }
    }
}
