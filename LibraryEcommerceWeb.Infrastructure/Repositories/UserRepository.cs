using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryEcommerceWeb.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository<User>
    {
        private AppDbContext _context { get; set; }

        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

    }
}
