using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;

namespace Blogers.Persistense.Repository
{
    public class FakeStoryRepository : IRepository<Story>
    {

        private readonly List<Story> _stories;

        public FakeStoryRepository()
        {
            _stories = new List<Story>();

            var s1 = new Story("24 часа челлендж: делаю лабы в бугор", 5, 25, 1);
            var s2 = new Story("Почему сладкое вредно", 15, 25, 1);
            var s3 = new Story("Мяумяумяу", 5, 25, 2);

            _stories.Add(s1);
            _stories.Add(s2);
            _stories.Add(s3);
        }

        public async Task<IReadOnlyList<Story>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => _stories, cancellationToken);
        }


        public async Task<Story?> GetByIdAsync(int id, CancellationToken cancellationToken = default, 
                                         params Expression<Func<Story, object>>[]? includesProperties)
        {
            var story = await Task.Run(() => _stories.FirstOrDefault(s => s.BlogerId == id), cancellationToken);
            return story;
        }

        public async Task<IReadOnlyList<Story>> ListAsync(Expression<Func<Story, bool>>? filter,
                                                    CancellationToken cancellationToken = default,
                                                    params Expression<Func<Story, object>>[]? includesProperties)
        {
            IQueryable<Story> stories = _stories.AsQueryable();

            if (includesProperties != null && includesProperties.Any())
            {
               
            }

            if (filter != null)
            {
                stories = stories.Where(filter);
            }

            return await Task.Run(() => stories.ToList(), cancellationToken);
        }


        public Task AddAsync(Story entity, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task UpdateAsync(Story entity, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task DeleteAsync(Story entity, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Story?> FirstOrDefaultAsync(Expression<Func<Story, bool>> filter, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }
}

