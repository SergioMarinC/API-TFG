using API_TFG.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace API_TFG.Repositories.UserRepositories
{
    public interface IUserRepository
    {
        Task<User?> CreateAsync(User user, string password, string[] roles);
        Task<string?> LoginAsync(User user, string password);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> UpdateAsync(Guid id, User user, string password);
        Task<User?> DeleteAsync(Guid id);
        Task<bool> CheckPassWordAsync(User user, string password);
    }
}
