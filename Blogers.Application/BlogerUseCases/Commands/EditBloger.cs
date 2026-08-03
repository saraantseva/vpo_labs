using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Application.BlogerUseCases.Commands
{

    public sealed record EditBlogerCommand(int Id, string Name, DateTime DateOfBirth,
    string nickname, int? followersNumber = 0) : IRequest<Bloger>;

    internal class EditBlogerHandler(IUnitOfWork unitOfWork) : IRequestHandler<EditBlogerCommand, Bloger>
    {
        public async Task<Bloger> Handle(EditBlogerCommand request, CancellationToken cancellationToken)
        {
            Bloger bloger = await unitOfWork.BlogerRepository.GetByIdAsync(request.Id);
            if (bloger == null) {
                return null;
            }

            bloger.UpdatePersonalData(new Person(request.Name, request.DateOfBirth));
            bloger.UpdateNickname(request.nickname);
            bloger.UpdateFollowersNumber(request.followersNumber);


            await unitOfWork.BlogerRepository.UpdateAsync(bloger, cancellationToken);
            await unitOfWork.SaveAllAsync();
            return bloger;
        }
    }
}
