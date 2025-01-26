using API_TFG.CustomActionFilters;
using API_TFG.Data;
using API_TFG.Models.Domain;
using API_TFG.Models.DTO.UserDtos;
using API_TFG.Repositories.UserRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace API_TFG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        //CREATE NEW USER
        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var user = mapper.Map<User>(registerRequestDto);

            user = await userRepository.CreateAsync(user, registerRequestDto.Password, registerRequestDto.Roles);

            if (user == null)
            {
                return BadRequest("No se ha podido crear el usuario");
            }
            
            return Ok(mapper.Map<UserDto>(user));

        }

        //LOGIN
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = mapper.Map<User>(loginRequestDto);

            var token = await userRepository.LoginAsync(user, loginRequestDto.Password);

            if (token != null)
            {
                LoginResponseDto loginResponseDto = new LoginResponseDto
                {
                    JwtToken = token
                };
                return Ok(loginResponseDto);
            }

            return BadRequest("Username or password incorrect");
        }

        //GET BY ID
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<UserDto>(user));
        }

        //UPDATE USER
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserRequestDto updateUserRequestDto)
        {
            // Obtener el ID del usuario autenticado desde el token
            var authenticatedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authenticatedUserId == null || !Guid.TryParse(authenticatedUserId, out Guid authenticatedUserGuid))
            {
                return Unauthorized("User ID not found or invalid.");
            }

            // Verificar que el usuario autenticado coincide con el ID solicitado
            if (authenticatedUserGuid != id)
            {
                return Forbid("You are not allowed to update another user's data.");
            }

            // Mapear el DTO al modelo de dominio
            var updatedUser = mapper.Map<User>(updateUserRequestDto);

            // Intentar actualizar el usuario
            var result = await userRepository.UpdateAsync(id, updatedUser, updateUserRequestDto.Password);

            if (result == null)
            {
                return NotFound("User not found.");
            }

            return Ok(mapper.Map<UserDto>(result));
        }

        //DELETE USER
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var authenticatedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authenticatedUserId == null || !Guid.TryParse(authenticatedUserId, out Guid authenticatedUserGuid))
            {
                return Unauthorized("User ID not found or invalid.");
            }

            // Verificar que el usuario autenticado coincide con el ID solicitado
            if (authenticatedUserGuid != id)
            {
                return Forbid("You are not allowed to update another user's data.");
            }

            var user = await userRepository.DeleteAsync(id);

            if (user == null)
            {
                return BadRequest("Failed to delete user");
            }

            return Ok(mapper.Map<UserDto>(user));
        }
    }
}
