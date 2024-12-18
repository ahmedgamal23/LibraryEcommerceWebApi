using LibraryEcommerceWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryEcommerceWeb.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure OrderItem -> Product relationship (restrict delete)
            /*
             1 - DeleteBehavior.Restrict:
                        Prevents the deletion of a Product if it's referenced in an OrderItem.
             2 - DeleteBehavior.Cascade:
                        Allows deletion of an Order and automatically removes associated OrderItems. 
             */
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure OrderItem -> Order relationship (default cascading is okay)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Default if you want cascading

            // Configure Review -> Customer relationship (default cascading is okay)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ShoppingCartItem -> Customer relationship (default cascading is okay)
            modelBuilder.Entity<ShoppingCartItem>()
                .HasOne(r => r.Customer)
                .WithMany(u => u.ShoppingCartItems)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
