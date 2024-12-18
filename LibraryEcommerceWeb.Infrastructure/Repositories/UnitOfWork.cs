using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Infrastructure.Data;

namespace LibraryEcommerceWeb.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IUserRepository<User> Users { get; private set; }
        public IGenericRepository<Category> Categories { get; private set; }
        public IGenericRepository<Discount> Discounts { get; private set; }
        public IGenericRepository<Product> Products { get; private set; }
        public IGenericRepository<Order> Orders { get; private set; }
        public IGenericRepository<OrderItem> OrderItems { get; private set; }
        public IGenericRepository<Payment> Payments { get; private set; }
        public IGenericRepository<ShoppingCartItem> ShoppingCartItems { get; private set; }
        public IGenericRepository<Review> Reviews { get; private set; }
        public IGenericRepository<Address> Addresses { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Categories = new GenericRepository<Category>(_context);
            Discounts = new GenericRepository<Discount>(_context);
            Products = new GenericRepository<Product>(_context);
            Orders = new GenericRepository<Order>(_context);
            OrderItems = new GenericRepository<OrderItem>(_context);
            Payments = new GenericRepository<Payment>(_context);
            ShoppingCartItems = new GenericRepository<ShoppingCartItem>(_context);
            Reviews = new GenericRepository<Review>(_context);
            Addresses = new GenericRepository<Address>(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
