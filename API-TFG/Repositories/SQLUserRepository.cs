using API_TFG.Data;
using API_TFG.Models.Domain;
using API_TFG.Models.DTO.UserDtos;
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

                        return tokenRepository.CreateJWTToken(userLogin, roles.ToList());

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

        //public async Task<List<User>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        //{
        //    var users = dbContext.Users.AsQueryable();

        //    //Filtering
        //    if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
        //    {
        //        if (filterOn.Equals("UserName", StringComparison.OrdinalIgnoreCase))
        //        {
        //            users = users.Where(x => x.UserName.Contains(filterQuery));
        //        }
        //        if (filterOn.Equals("email", StringComparison.OrdinalIgnoreCase))
        //        {
        //            users = users.Where(x => x.Email.Contains(filterQuery));
        //        }
        //    }

        //    //Sorting
        //    if (string.IsNullOrWhiteSpace(sortBy) == false)
        //    {
        //        if (sortBy.Equals("username", StringComparison.OrdinalIgnoreCase))
        //        {
        //            users = isAscending ? users.OrderBy(x => x.UserName) : users.OrderByDescending(x => x.UserName);
        //        }
        //        else if (sortBy.Equals("email", StringComparison.OrdinalIgnoreCase))
        //        {
        //            users = isAscending ? users.OrderBy(x => x.Email) : users.OrderByDescending(x => x.Email);
        //        }
        //    }

        //    //Pagination
        //    var skipResults = (pageNumber - 1) * pageSize;

        //    return await users.Skip(skipResults).Take(pageSize).ToListAsync();
        //}

        //public async Task<User?> GetByEmailAsync(string email)
        //{
        //    return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        //}



        //public async Task<User?> GetByUsernameAsync(string username)
        //{
        //    return await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
        //}




    }
}
