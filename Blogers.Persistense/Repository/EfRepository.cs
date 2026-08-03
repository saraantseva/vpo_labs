using Microsoft.EntityFrameworkCore;
using Blogers.Persistense.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Blogers.Persistense.Repository
{
    public class EfRepository<T> : IRepository<T> where T : Entity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _entities;

        public EfRepository(AppDbContext dbContext){
            _context = dbContext;
            _entities = _context.Set<T>();
        }

        public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> filter,
         CancellationToken cancellationToken = default,
         params Expression<Func<T, object>>[]? includesProperties)
        {
            IQueryable<T>? query = _entities.AsQueryable();
            if (includesProperties.Any())
            {
                foreach (Expression<Func<T, object>>? included in includesProperties)
                {
                    query = query.Include(included);
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync(cancellationToken);

        }

        public Task UpdateAsync(T entity,
            CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public async Task<T>? GetByIdAsync(int id,
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[]? includesProperties)
        {
            IQueryable<T> query =  _entities.AsQueryable();
            if (includesProperties.Any())
            {
                foreach(Expression<Func<T, object>> include in includesProperties)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }
        public async Task<IReadOnlyList<T>> ListAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _entities.ToListAsync(cancellationToken);
        }

        public async Task AddAsync(T entity,
            CancellationToken cancellationToken = default)
        {
            await _entities.AddAsync(entity);
        }
        public async Task DeleteAsync(T entity,
            CancellationToken cancellationToken = default)
        {
            var existing = await _entities.FindAsync(entity.Id, cancellationToken);
            if (existing != null)
            {
                _entities.Remove(existing);
            }
        }

        public async Task<T> FirstOrDefaultAsync(
         Expression<Func<T, bool>> filter,
         CancellationToken cancellationToken = default)
        {
            IQueryable<T>? query = _entities.AsQueryable();
            if(filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

    }
}
