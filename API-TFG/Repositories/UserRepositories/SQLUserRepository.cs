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

            // Eliminar archivos asociados al usuario de manera manual
            var filesToDelete = appDbContext.Files.Where(f => f.Owner.Id == id).ToList();
            if (filesToDelete.Any())
            {
                appDbContext.Files.RemoveRange(filesToDelete);
                await appDbContext.SaveChangesAsync(); // Asegúrate de guardar los cambios antes de eliminar al usuario
            }

            // Eliminar el usuario
            var result = await userManager.DeleteAsync(existingUser);
            if (result.Succeeded)
            {
                return existingUser;
            }

            return null;
        }


        public async Task<bool> CheckPassWordAsync(User user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }
    }
}
