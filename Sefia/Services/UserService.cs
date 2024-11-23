using Microsoft.EntityFrameworkCore;
using Sefia.Data;
using Sefia.Dtos;
using Sefia.Entities;
using Sefia.Utils;

namespace Sefia.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // Get user by ID
        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        // Get user by email
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        // Add new user
        public async Task<User> AddUserAsync(string email, string password, string name)
        {
            // Check if email already exists
            var existingUser = await GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("This email address is already in use. Please try a different one");
            }

            // Hash the password and create a new user
            var pwHash = PasswordHasher.HashPassword(password);
            var user = new User(email, pwHash, name);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Delete user by ID
        public async Task DeleteUserAsync(string id)
        {
            var existingUser = await GetUserByIdAsync(id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            _context.Users.Remove(existingUser);
            await _context.SaveChangesAsync();
        }
    }
}
