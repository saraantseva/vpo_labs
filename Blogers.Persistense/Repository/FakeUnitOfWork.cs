using System;
using System.Collections.Generic;
using System.Text;
using IUnitOfWork = Blogers.Domain.Abstractions.IUnitOfWork;

namespace Blogers.Persistense.Repository
{
    public class FakeUnitOfWork : IUnitOfWork
    {
        public IRepository<Bloger> BlogerRepository { get; }
        public IRepository<Story> StoryRepository { get; }

        public FakeUnitOfWork()
        {
            BlogerRepository = new FakeBlogerRepository();
            StoryRepository = new FakeStoryRepository();
        }

        public Task SaveAllAsync()
        {
            return Task.CompletedTask;
        }

        public Task DeleteDataBaseAsync() => Task.CompletedTask;
        public Task CreateDataBaseAsync() => Task.CompletedTask;


    }
}
