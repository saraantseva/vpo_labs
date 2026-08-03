using Abp.Domain.Uow;
using Blogers.Persistense.Data;
using System;
using System.Collections.Generic;
using System.Text;
using IUnitOfWork = Blogers.Domain.Abstractions.IUnitOfWork;

namespace Blogers.Persistense.Repository
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly Lazy<IRepository<Bloger>> _blogerRepository;
        private readonly Lazy<IRepository<Story>> _storyRepository;

        public EfUnitOfWork(AppDbContext context)
        {
            _context = context;
            _blogerRepository = new Lazy<IRepository<Bloger>>(() =>
                new EfRepository<Bloger>(context));
            _storyRepository = new Lazy<IRepository<Story>>(() =>
                new EfRepository<Story>(context));
        }
        public IRepository<Bloger> BlogerRepository
            => _blogerRepository.Value;
        public IRepository<Story> StoryRepository
            => _storyRepository.Value;

        public async Task SaveAllAsync() =>
         await _context.SaveChangesAsync();

        public async Task CreateDataBaseAsync() =>
         await _context.Database.EnsureCreatedAsync();
        public async Task DeleteDataBaseAsync() =>
         await _context.Database.EnsureDeletedAsync();

    }

}
