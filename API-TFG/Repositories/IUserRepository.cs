using API_TFG.Models.Domain;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace API_TFG.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync(string? filterOn = null, string? filterQuery = null);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> CreateAsync(User user);
        Task<User?> UpdateAsync(Guid id, User user);
        Task<User?> DeleteAsync(Guid id);
        Task<User?> GetByUsernameAsync(String username);
        Task<User?> GetByEmailAsync(String email);
    }
}
