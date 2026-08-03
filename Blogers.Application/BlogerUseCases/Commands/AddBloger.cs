using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Blogers.Application.BlogerUseCases.Commands
{
    public sealed record AddBlogerCommand(string Name, DateTime DateOfBirth,
        string nickname, int? followersNumber = 0) : IRequest<Bloger>;

    internal class AddBlogerHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddBlogerCommand, Bloger>
    {
        public async Task<Bloger> Handle(AddBlogerCommand request, CancellationToken cancellationToken)
        {
            Bloger bloger = new Bloger(new Person(request.Name, request.DateOfBirth),
                request.nickname, request.followersNumber ?? 0);

            await unitOfWork.BlogerRepository.AddAsync(bloger, cancellationToken);
            await unitOfWork.SaveAllAsync();
            return bloger;
        }
    }
}
