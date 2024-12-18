namespace LibraryEcommerceWeb.Application.Interfaces
{
    public interface IUserRepository<T> : IGenericRepository<T> where T : class
    {
        Task<T?> GetByEmailAsync(string email);
    }
}
