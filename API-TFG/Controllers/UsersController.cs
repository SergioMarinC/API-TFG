using API_TFG.Data;
using API_TFG.Models.Domain;
using API_TFG.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_TFG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        public UsersController(AppDbContext dbContext)
        {
            this.dbContext = dbContext; 
        }

        //GET ALL USERS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await dbContext.Users.ToListAsync();

            var usersDto = new List<UserDto>();

            foreach (var user in users)
            {
                usersDto.Add(new UserDto
                {
                    UserID = user.UserID,
                    Username = user.Username,
                    Email = user.Email,
                    Password = user.PasswordHash,
                    CreatedDate = user.CreatedDate,
                    LastLogin = user.LastLogin
                });
            }
            return Ok(usersDto);
        }

        //GET USER ID
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //var user = dbContext.Users.FirstOrDefault(x => x.UserID == id);
            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email,
                Password = user.PasswordHash,
                CreatedDate = user.CreatedDate,
                LastLogin = user.LastLogin
            };

            return Ok(userDto);
        }

        //CREATE NEW USER
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddUserRequestDto addUserRequestDto)
        {
            var user = new User
            {
                Username = addUserRequestDto.Username,
                Email = addUserRequestDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(addUserRequestDto.Password)
            };

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            var userDto = new UserDto
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email,
                Password = user.PasswordHash,
                CreatedDate = user.CreatedDate,
                LastLogin = user.LastLogin

            };

            return CreatedAtAction(nameof(GetById), new { id = userDto.UserID}, userDto);
        }

        //UPDATE USER
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserRequestDto updateUserRequestDto)
        {
            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.Username = updateUserRequestDto.Username;
            user.Email = updateUserRequestDto.Email;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserRequestDto.Password);

            await dbContext.SaveChangesAsync();

            var userDto = new UserDto
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email,
                Password = user.PasswordHash,
                CreatedDate = user.CreatedDate,
                LastLogin = user.LastLogin
            };

            return Ok(userDto);
        }

        //DELETE USER
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id) 
        {
            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();

            var userDto = new UserDto
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email,
                Password = user.PasswordHash,
                CreatedDate = user.CreatedDate,
                LastLogin = user.LastLogin
            };

            return Ok(userDto);
        }
    }
}
