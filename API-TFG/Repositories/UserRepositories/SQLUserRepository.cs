using API_TFG.Data;
using API_TFG.Models.Domain;
using API_TFG.Models.DTO.UserDtos;
using API_TFG.Repositories.TokenRepositories;
using API_TFG.Repositories.UserRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Identity.Client;
using System.Globalization;

namespace API_TFG.Repositories
{
    public class SQLUserRepository : IUserRepository
    {
        private readonly UserManager<User> userManager;
        private readonly ITokenRepository tokenRepository;
        private readonly AppDbContext appDbContext;

        public SQLUserRepository(UserManager<User> userManager, ITokenRepository tokenRepository, AppDbContext appDbContext)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
            this.appDbContext = appDbContext;
        }

        public async Task<User?> CreateAsync(User user, string password, string[] roles)
        {
            var identityResult =  await userManager.CreateAsync(user, password);

            if (identityResult.Succeeded)
            {
                //Add roles to this User
                if (roles != null && roles.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(user, roles);

                    
                    if (!identityResult.Succeeded)
                    {
                        await userManager.DeleteAsync(user);
                        return null;
                    }
                }
                var createdUser = await userManager.FindByEmailAsync(user.Email);
                return createdUser;
            }
            return null;
        }

        public async Task<string?> LoginAsync(User user, string password)
        {
            var userLogin = await userManager.FindByEmailAsync(user.Email);

            if (userLogin != null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(userLogin, password);

                if (checkPasswordResult)
                {
                    //Get Roles for this user
                    var roles = await userManager.GetRolesAsync(userLogin);

                    if (roles != null && roles.Any())
                    {
                        //Create Token

                        return await tokenRepository.CreateJWTToken(userLogin, roles.ToList());

                    }
                }
            }
            return null;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await userManager.FindByIdAsync(id.ToString());
        }

        public async Task<User?> UpdateAsync(Guid id, User user, string password)
        {
            var existingUser = await GetByIdAsync(id);

            if (existingUser == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(user.UserName))
            {
                existingUser.UserName = user.UserName;
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                existingUser.Email = user.Email;
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                var removePasswordResult = await userManager.RemovePasswordAsync(existingUser);
                if (!removePasswordResult.Succeeded)
                {
                    return null;
                }

                var addPasswordResult = await userManager.AddPasswordAsync(existingUser, password);
                if (!addPasswordResult.Succeeded)
                {
                    return null;
                }
            }
            var updateResult = await userManager.UpdateAsync(existingUser);
            if (!updateResult.Succeeded)
            {
                return null;
            }

            return existingUser; // Devolver el usuario actualizado
        }

        public async Task<User?> DeleteAsync(Guid id)
        {
            var existingUser = await GetByIdAsync(id);
            if (existingUser == null)
            {
                return null;
            }

            if (existingUser.Files != null && existingUser.Files.Any())
            {
                foreach (var file in existingUser.Files)
                {
                    appDbContext.Files.Remove(file);
                }
            }
            var result = await userManager.DeleteAsync(existingUser);
            if (result.Succeeded)
            {
                return existingUser;
            }
            return null;
        }

        public Task<User?> GetByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}
