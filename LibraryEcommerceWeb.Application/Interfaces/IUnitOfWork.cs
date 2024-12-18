using LibraryEcommerceWeb.Domain.Entities;

namespace LibraryEcommerceWeb.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // write all repositories and other functions need in Unit of work class
        IUserRepository<User> Users { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Discount> Discounts { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        IGenericRepository<Payment> Payments { get; }
        IGenericRepository<ShoppingCartItem> ShoppingCartItems { get; }
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<Address> Addresses { get; }

        Task<int> SaveChangesAsync();
    }
}
