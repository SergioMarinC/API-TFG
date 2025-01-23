using API_TFG.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace API_TFG.Repositories
{
    public interface IUserRepository
    {
        Task<User?> CreateAsync(User user, string password, string[] roles);
        Task<string?> LoginAsync(User user, string password);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> UpdateAsync(Guid id, User user, string password);
        Task<User?> DeleteAsync(Guid id);
        //Task<List<User>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        Task<User?> GetByUsernameAsync(String username);
        Task<User?> GetByEmailAsync(String email);
    }
}
