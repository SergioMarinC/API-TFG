﻿using API_TFG.Data;
using API_TFG.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace API_TFG.Repositories
{
    public class SQLUserRepository : IUserRepository
    {
        private readonly AppDbContext dbContext;

        public SQLUserRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<User?> CreateAsync(User user)
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User?> DeleteAsync(Guid id)
        {
            var existingUser = await dbContext.Users.FindAsync(id);
            if (existingUser == null)
            {
                return null;
            }

            dbContext.Users.Remove(existingUser);
            await dbContext.SaveChangesAsync();
            return existingUser;
        }

        public async Task<List<User>> GetAllAsync(string? filterOn = null, string? filterQuery = null)
        {
            var users = dbContext.Users.AsQueryable();

            //Filtering
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("Username", StringComparison.OrdinalIgnoreCase))
                {
                    users = users.Where(x => x.Username.Contains(filterQuery));
                }
                if (filterOn.Equals("email", StringComparison.OrdinalIgnoreCase))
                {
                    users = users.Where(x => x.Email.Contains(filterQuery));
                }
            }

            return await users.ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await dbContext.Users.FindAsync(id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> UpdateAsync(Guid id, User user)
        {
            var existingUser = await dbContext.Users.FindAsync(id);

            if (existingUser == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(user.Username))
            {
                existingUser.Username = user.Username;
            }

            if (!string.IsNullOrEmpty(user.Username))
            {
                existingUser.Username = user.Username;
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                existingUser.Email = user.Email;
            }

            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                existingUser.PasswordHash = user.PasswordHash;
            }

            await dbContext.SaveChangesAsync();
            return existingUser;
        }
    }
}
