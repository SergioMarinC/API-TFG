using API_TFG.Data;
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

        public async Task<User> CreateAsync(User user)
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

        public async Task<List<User>> GetAllAsync()
        {
           return await dbContext.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await dbContext.Users.FindAsync(id);
        }

        public async Task<User?> UpdateAsync(Guid id, User user)
        {
            var existingUser = await dbContext.Users.FindAsync(id);

            if (existingUser == null)
            {
                return null;
            }

            existingUser.Username = user.Username;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.Email = user.Email;
            
            await dbContext.SaveChangesAsync();
            return existingUser;
        }
    }
}
