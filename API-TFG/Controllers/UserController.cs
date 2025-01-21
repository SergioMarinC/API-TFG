using API_TFG.CustomActionFilters;
using API_TFG.Data;
using API_TFG.Models.Domain;
using API_TFG.Models.DTO;
using API_TFG.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_TFG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;

        public UserController(IUserRepository userRepository, IMapper mapper, UserManager<User> userManager)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        //GET ALL USERS
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var users = await userRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);

            return Ok(mapper.Map<List<UserDto>>(users));
        }

        //GET USER ID
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //var user = dbContext.Users.FirstOrDefault(x => x.UserID == id);
            var user = await userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<UserDto>(user));
        }

        ////CREATE NEW USER
        //[HttpPost]
        //[ValidateModel]
        //public async Task<IActionResult> Create([FromBody] AddUserRequestDto addUserRequestDto)
        //{
        //    if (await userRepository.GetByUsernameAsync(addUserRequestDto.Username) != null)
        //    {
        //        return Conflict("The Username already exists in the database");
        //    }
        //    if (await userRepository.GetByEmailAsync(addUserRequestDto.Email) != null)
        //    {
        //        return Conflict("The Email already exists in the database");
        //    }
        //    var user = mapper.Map<User>(addUserRequestDto);

        //    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(addUserRequestDto.Password);

        //    user = await userRepository.CreateAsync(user);

        //    var userDto = mapper.Map<UserDto>(user);

        //    return CreatedAtAction(nameof(GetById), new { id = userDto.UserID }, userDto);
        //}

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] AddUserRequestDto requestDto)
        {
            var identityUser = new User
            {
                UserName = requestDto.Username,
                Email = requestDto.Username,
            };

            var identityResult = await userManager.CreateAsync(identityUser, requestDto.Password);

            if (identityResult.Succeeded)
            {
                //Add roles to this User
                if (requestDto.Roles != null && requestDto.Roles.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(identityUser, requestDto.Roles);

                    if (identityResult.Succeeded)
                    {
                        return Ok("User was registered");
                    }
                }

            }
            return BadRequest("Something went wrong");
        }

        //UPDATE USER
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserRequestDto updateUserRequestDto)
        {
            var existingUser = await userRepository.GetByIdAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            // Mapear solo los campos no nulos del DTO al modelo existente
            if (!string.IsNullOrEmpty(updateUserRequestDto.Username))
            {
                existingUser.UserName = updateUserRequestDto.Username;
            }

            if (!string.IsNullOrEmpty(updateUserRequestDto.Email))
            {
                existingUser.Email = updateUserRequestDto.Email;
            }

            if (!string.IsNullOrEmpty(updateUserRequestDto.Password))
            {
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserRequestDto.Password);
            }

            // Actualizar en la base de datos
            var updatedUser = await userRepository.UpdateAsync(id, existingUser);

            return Ok(mapper.Map<UserDto>(updatedUser));
        }

        //DELETE USER
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var user = await userRepository.DeleteAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<UserDto>(user));
        }
    }
}
