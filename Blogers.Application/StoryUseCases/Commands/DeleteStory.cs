using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Application.StoryUseCases.Commands
{
    public sealed record DeleteStoryCommand(int Id) : IRequest;

    internal class DeleteStoryHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteStoryCommand>
    {
        public async Task Handle(DeleteStoryCommand request, CancellationToken cancellationToken)
        {
            var story = await unitOfWork.StoryRepository.GetByIdAsync(request.Id, cancellationToken);

            if (story != null)
            {
                await unitOfWork.StoryRepository.DeleteAsync(story, cancellationToken);
                await unitOfWork.SaveAllAsync();
            }
        }
    }
}
