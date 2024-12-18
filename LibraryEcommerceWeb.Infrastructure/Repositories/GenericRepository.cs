using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryEcommerceWeb.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;

        public GenericRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (include != null)
            {
                query = include(query);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T item)
        {
            await _context.Set<T>().AddAsync(item);
        }

        public void Update(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Cannot update a null entity.");
            _context.Set<T>().Update(item);
        }

        public async Task DeleteAsync(string id)
        {
            var item = await GetByIdAsync(id);
            if (item != null)
            {
                _context.Set<T>().Remove(item);
            }
        }

    }
}


