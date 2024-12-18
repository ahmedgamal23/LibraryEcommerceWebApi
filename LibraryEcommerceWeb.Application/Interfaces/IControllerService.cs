using LibraryEcommerceWeb.Shared;

namespace LibraryEcommerceWeb.Application.Interfaces
{
    public interface IControllerService<T> where T : class
    {
        Task<StateResponse> GetAllAsync(int pageNumber, int pageSize);
        Task<StateResponse> GetByIdAsync(string id);
        Task<StateResponse> CreateAsync(T item);
        Task<StateResponse> UpdateAsync(string id, T item);
        Task<StateResponse> DeleteAsync(string id);
    }
}
