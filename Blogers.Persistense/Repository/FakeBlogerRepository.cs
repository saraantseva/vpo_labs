using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Blogers.Persistense.Repository
{
    [ExcludeFromCodeCoverage]
    public class FakeBlogerRepository : IRepository<Bloger>
    {
        private readonly List<Bloger> _blogers;

        public FakeBlogerRepository()
        {
            _blogers = new List<Bloger>();

            var p1 = new Person("Stas", DateTime.Parse("28-08-2006"));
            var b1 = new Bloger (p1, "Stasnames", 31);
            b1.Id = 1;


            var p2 = new Person("Albina", DateTime.Parse("04-04-2004"));
            var b2 = new Bloger(p2, "porphyriacus", 1000000);
            b2.Id = 2;

            _blogers.Add(b1);
            _blogers.Add(b2);
        }

        public async Task<IReadOnlyList<Bloger>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => _blogers, cancellationToken);
        }

 
        public Task<Bloger?> GetByIdAsync(int id, CancellationToken cancellationToken = default, params Expression<Func<Bloger, object>>[]? includesProperties)
            => throw new NotImplementedException();

        public Task<IReadOnlyList<Bloger>> ListAsync(Expression<Func<Bloger, bool>>? filter, CancellationToken cancellationToken = default, params Expression<Func<Bloger, object>>[]? includesProperties)
            => throw new NotImplementedException();

        public Task AddAsync(Bloger entity, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task UpdateAsync(Bloger entity, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task DeleteAsync(Bloger entity, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Bloger?> FirstOrDefaultAsync(Expression<Func<Bloger, bool>> filter, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }
}
