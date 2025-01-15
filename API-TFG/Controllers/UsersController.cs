using API_TFG.CustomActionFilters;
using API_TFG.Data;
using API_TFG.Models.Domain;
using API_TFG.Models.DTO;
using API_TFG.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_TFG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        //GET ALL USERS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await userRepository.GetAllAsync();

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

        //CREATE NEW USER
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddUserRequestDto addUserRequestDto)
        {
            var user = mapper.Map<User>(addUserRequestDto);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(addUserRequestDto.Password);

            user = await userRepository.CreateAsync(user);

            var userDto = mapper.Map<UserDto>(user);

            return CreatedAtAction(nameof(GetById), new { id = userDto.UserID }, userDto);
        }

        //UPDATE USER
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserRequestDto updateUserRequestDto)
        {
            var user = mapper.Map<User>(updateUserRequestDto);

            user = await userRepository.UpdateAsync(id, user);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<UserDto>(user));
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
