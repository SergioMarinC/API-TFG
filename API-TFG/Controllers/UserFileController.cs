using API_TFG.Data;
using API_TFG.Models.Domain;
using API_TFG.Models.DTO;
using API_TFG.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_TFG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserFileController : ControllerBase
    {
        private readonly IUserFileRepository userFileRepository;
        private readonly IMapper mapper;

        public UserFileController(IUserFileRepository userFileRepository, IMapper mapper)
        {
            this.userFileRepository = userFileRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("user/{userId:Guid}")]
        public async Task<IActionResult> GetSharedFilesByUser(Guid userId)
        {
            var userFiles = await userFileRepository.GetFilesSharedWithUserAsync(userId);

            if (userFiles.Any())
            {
                return NotFound("No hay archivos compartidos para este usuario");
            }

            return Ok(mapper.Map<List<UserFileDto>>(userFiles));

        }

        [HttpGet]
        [Route("{userId:guid}/{fileId:guid}")]
        public async Task<IActionResult> GetUserFileAccess(Guid userId, Guid fileId)
        {
            var userFileAccess = await userFileRepository.GetUserFileAccessAsync(userId, fileId);
            if (userFileAccess == null)
            {
                return NotFound(new { Message = "Acceso no encontrado para el archivo y usuario proporcionados." });
            }

            return Ok(mapper.Map<UserFileDto>(userFileAccess));
        }

        [HttpGet]
        [Route("{fileId:guid}")]
        public async Task<IActionResult> GetUsersWithAccessToFile(Guid fileId)
        {
            var usersWithAccess = await userFileRepository.GetUserWithAccesToFileAsync(fileId);

            if (usersWithAccess.Any())
            {
                return NotFound("File is not shared with anyone");
            }

            return Ok(mapper.Map<List<UserDto>>(usersWithAccess));
        }

        [HttpPost]
        public async Task<IActionResult> ShareFile([FromForm] ShareFileDto addUserFileDto)
        {
            if (ModelState.IsValid)
            {
                var userFile = mapper.Map<UserFile>(addUserFileDto);

                var createdUserFile = await userFileRepository.CreateAsync(userFile);

                return Ok(mapper.Map<ShareFileDto>(createdUserFile));
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut]
        [Route("{userFileId}/permissions")]
        public async Task<IActionResult> UpdateFilePermissions(int userFileId, [FromBody] UpdateUserFileRequestDto updateUserFileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedUserFile = await userFileRepository.UpdatePermissionsAsync(userFileId, updateUserFileDto.PermissionType);

            if (updatedUserFile == null)
            {
                return NotFound($"No UserFile found with ID {userFileId}");
            }

            return Ok(mapper.Map<UserFileDto>(updatedUserFile));
        }

        [HttpDelete]
        [Route("{userFileId}/revoke")]
        public async Task<IActionResult> RevokeFileAccess(int userFileId)
        {
            var revokeUserFile = await userFileRepository.RemoveUserAccessAsync(userFileId);

            if (revokeUserFile == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(mapper.Map<UserFileDto>(revokeUserFile));
            }
        }
    }
}
