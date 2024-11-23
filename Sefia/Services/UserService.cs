using Microsoft.EntityFrameworkCore;
using Sefia.Common;
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

        /// <summary>
        /// Get User by ID, if user not found, return null
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        /// <summary>
        /// Get User by email, if user not found, return null
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        /// <summary>
        /// Add new User, Not Admin
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
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

        /// <summary>
        /// Deletes a user, Admin users are not allowed to be deleted.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task DeleteUserAsync(string id)
        {
            var existingUser = await GetUserByIdAsync(id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            if (existingUser.Role == UserRoles.Admin)
            {
                throw new InvalidOperationException("Admin users cannot be deleted.");
            }

            _context.Users.Remove(existingUser);
            await _context.SaveChangesAsync();
        }
    }
}
