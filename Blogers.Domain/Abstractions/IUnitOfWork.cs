using Blogers.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        IRepository<Bloger> BlogerRepository { get; }
        IRepository<Story> StoryRepository { get; }
        public Task SaveAllAsync();
        public Task DeleteDataBaseAsync();
        public Task CreateDataBaseAsync();


    }
}
