namespace LibraryEcommerceWeb.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<T?> GetByIdAsync(string id);
        Task AddAsync(T item);
        void Update(T item);
        Task DeleteAsync(string id);
    }
}
