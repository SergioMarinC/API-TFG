using API_TFG.CustomActionFilters;
using API_TFG.Data;
using API_TFG.Models.Domain;
using API_TFG.Models.DTO;
using API_TFG.Models.Enum;
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
        private readonly IAuditLogRepository auditLogRepository;
        private readonly IMapper mapper;

        public UserFileController(IUserFileRepository userFileRepository, IAuditLogRepository auditLogRepository, IMapper mapper)
        {
            this.userFileRepository = userFileRepository;
            this.auditLogRepository = auditLogRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// Get shared files by user Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of UserFiles</returns>
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

        /// <summary>
        /// Get user file access for an file by id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileId"></param>
        /// <returns>UserFile</returns>
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

        /// <summary>
        /// Get all the users with access to a file by id
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns>List of users</returns>
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

        /// <summary>
        /// Shares a file to a user
        /// </summary>
        /// <param name="addUserFileDto"></param>
        /// <returns>The sharedFile</returns>
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> ShareFile([FromForm] ShareFileDto addUserFileDto)
        {
            var userFile = mapper.Map<UserFile>(addUserFileDto);
            
            var createdUserFile = await userFileRepository.CreateAsync(userFile);

            //AuditLog
            var auditLog = mapper.Map<AuditLog>(userFile);
            auditLog.Action = ActionType.Share;
            await auditLogRepository.CreateAuditLogAsync(auditLog);

            return Ok(mapper.Map<ShareFileDto>(createdUserFile));
        }

        /// <summary>
        /// Updates the perms for the sharedfile
        /// </summary>
        /// <param name="userFileId"></param>
        /// <param name="updateUserFileDto"></param>
        /// <returns>The updated shared file</returns>
        [HttpPut]
        [Route("{userFileId}/permissions")]
        [ValidateModel]
        public async Task<IActionResult> UpdateFilePermissions(int userFileId, [FromBody] UpdateUserFileRequestDto updateUserFileDto)
        {
            var updatedUserFile = await userFileRepository.UpdatePermissionsAsync(userFileId, updateUserFileDto.PermissionType);

            if (updatedUserFile == null)
            {
                return NotFound($"No UserFile found with ID {userFileId}");
            }

            //AuditLog
            var auditLog = mapper.Map<AuditLog>(updatedUserFile);
            auditLog.Action = ActionType.UpdatePermissions;
            await auditLogRepository.CreateAuditLogAsync(auditLog);

            return Ok(mapper.Map<UserFileDto>(updatedUserFile));
        }

        /// <summary>
        /// Revoke access to a file
        /// </summary>
        /// <param name="userFileId"></param>
        /// <returns>The revoked shared file</returns>
        [HttpDelete]
        [Route("{userFileId}/revoke")]
        public async Task<IActionResult> RevokeFileAccess(int userFileId)
        {
            var revokeUserFile = await userFileRepository.RemoveUserAccessAsync(userFileId);

            if (revokeUserFile == null)
            {
                return NotFound();
            }

            //AuditLog
            var auditLog = mapper.Map<AuditLog>(revokeUserFile);
            auditLog.Action = ActionType.RevokeAccess;
            await auditLogRepository.CreateAuditLogAsync(auditLog);

            return Ok(mapper.Map<UserFileDto>(revokeUserFile));
        }
    }
}
