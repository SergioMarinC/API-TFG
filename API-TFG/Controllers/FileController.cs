﻿using API_TFG.CustomActionFilters;
using API_TFG.Data;
using API_TFG.Models.Domain;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using API_TFG.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using API_TFG.Repositories.UserRepositories;
using API_TFG.Repositories.FileRepositories;
using API_TFG.Repositories.AuditLogRepositories;
using API_TFG.Repositories.TokenRepositories;
using System.Security.Claims;
using API_TFG.Models.DTO.FileDtos;

namespace API_TFG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly IFileRepository fileRepository;
        private readonly IAuditLogRepository auditLogRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public FileController(IFileRepository fileRepository, IAuditLogRepository auditLogRepository, IMapper mapper, IUserRepository userRepository)
        {
            this.fileRepository = fileRepository;
            this.auditLogRepository = auditLogRepository;
            this.mapper = mapper;
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Get file by ID of the file
        /// </summary>
        /// <param name="id"></param>
        /// <returns>File</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            // Obtener el ID del usuario autenticado desde el token
            var authenticatedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authenticatedUserId == null || !Guid.TryParse(authenticatedUserId, out Guid authenticatedUserGuid))
            {
                return Unauthorized("User ID not found or invalid.");
            }

            //Obtiene el file
            var file = await fileRepository.GetByIdAsync(id);

            // Verificar que el usuario autenticado coincide con el ID del propietario o que lo tiene compartido
            if (authenticatedUserGuid != file.Owner.Id || (file.Owner.Id != authenticatedUserGuid && !file.SharedWithUsers.Any(swu => swu.User.Id == authenticatedUserGuid)))
            {
                return Forbid("You are not allowed to update another user's data.");
            }

            if (file == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<FileDto>(file));
        }

        /// <summary>
        /// Gets all the files by the ID of the User
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of Files</returns>
        [HttpGet]
        [Route("user/{id:Guid}")]
        public async Task<IActionResult> GetByUserId([FromRoute] Guid id, [FromQuery] string? filterOn, [FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var authenticatedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authenticatedUserId == null || !Guid.TryParse(authenticatedUserId, out Guid authenticatedUserGuid))
            {
                return Unauthorized("User ID not found or invalid.");
            }

            // Verificar que el usuario autenticado coincide con el ID del propietario
            if (authenticatedUserGuid != id)
            {
                return Forbid("You are not allowed to update another user's data.");
            }
            var files = await fileRepository.GetAllByUserIdAsync(id, filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);

            if (files == null)
            {
                return NotFound("User not found");
            }

            return Ok(mapper.Map<List<FileDto>>(files));
        }

        /// <summary>
        /// Creates a file and uploads the real file to the folder
        /// </summary>
        /// <param name="fileUploadDto"></param>
        /// <returns>File</returns>
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadDto fileUploadDto)
        {
            var file = mapper.Map<Models.Domain.File>(fileUploadDto);
            var owner = await userRepository.GetByIdAsync(fileUploadDto.OwnerID);

            // Obtener el ID del usuario autenticado desde el token
            var authenticatedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authenticatedUserId == null || !Guid.TryParse(authenticatedUserId, out Guid authenticatedUserGuid))
            {
                return Unauthorized("User ID not found or invalid.");
            }

            // Verificar que el usuario autenticado coincide con el ID del propietario
            if (authenticatedUserGuid != owner.Id)
            {
                return Forbid("You are not allowed to update another user's data.");
            }

            if (owner == null) 
            {
                return NotFound("The user doesn't exist.");
            }

            file.Owner = owner;

            var savedFile = await fileRepository.UploadAsync(file, fileUploadDto.UploadedFile);

            //AuditLog
            var auditLog = mapper.Map<AuditLog>(savedFile);

            auditLog.Action = ActionType.Upload;

            await auditLogRepository.CreateAuditLogAsync(auditLog);

            return Ok(mapper.Map<FileDto>(savedFile));
        }

        /// <summary>
        /// Updates the name and the folder path of a existing File, it changes the place of it in the folder
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateFileRequestDto"></param>
        /// <returns>The updated File</returns>
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateFileRequestDto updateFileRequestDto)
        {

            var updatedFile = mapper.Map<Models.Domain.File>(updateFileRequestDto);

            var owner = await userRepository.GetByIdAsync(id);

            var authenticatedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authenticatedUserId == null || !Guid.TryParse(authenticatedUserId, out Guid authenticatedUserGuid))
            {
                return Unauthorized("User ID not found or invalid.");
            }

            // Verificar que el usuario autenticado coincide con el ID del propietario
            if (authenticatedUserGuid != owner.Id)
            {
                return Forbid("You are not allowed to update another user's data.");
            }

            var file = await fileRepository.UpdateAsync(id, updatedFile);

            if (file == null)
            {
                return NotFound();
            }

            //AuditLog
            var auditLog = mapper.Map<AuditLog>(file);

            auditLog.Action = ActionType.Update;

            await auditLogRepository.CreateAuditLogAsync(auditLog);

            return Ok(mapper.Map<FileDto>(file));
        }

        /// <summary>
        /// Download the File
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Real File from the folder</returns>
        [HttpGet]
        [Route("download/{id:Guid}")]
        public async Task<IActionResult> DownloadFile([FromRoute] Guid id)
        {
            // Obtener el ID del usuario autenticado desde el token
            var authenticatedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authenticatedUserId == null || !Guid.TryParse(authenticatedUserId, out Guid authenticatedUserGuid))
            {
                return Unauthorized("User ID not found or invalid.");
            }

            // Obtiene el archivo
            var fileCheck = await fileRepository.GetByIdAsync(id);

            // Verificar que el usuario autenticado coincide con el ID del propietario o que lo tiene compartido
            if (authenticatedUserGuid != fileCheck.Owner.Id && (fileCheck.SharedWithUsers == null || !fileCheck.SharedWithUsers.Any(swu => swu.User.Id == authenticatedUserGuid)))
            {
                return Forbid("You are not allowed to access this file.");
            }

            // Llamar al repositorio para obtener el archivo y su contenido
            var (file, fileContent) = await fileRepository.DownloadAsync(id);

            if (file == null)
            {
                return NotFound("File not found in the database.");
            }

            if (fileContent == null)
            {
                return NotFound("File does not exist on the server.");
            }

            // Audit log
            var auditLog = mapper.Map<AuditLog>(file);
            auditLog.Action = ActionType.Download;

            await auditLogRepository.CreateAuditLogAsync(auditLog);

            // Determinar el tipo MIME
            var contentType = fileRepository.GetContentType(file.FilePath);

            // Devolver el archivo como respuesta
            return File(fileContent, contentType, file.FileName);
        }



        /// <summary>
        /// Changes the state of the file to "deleted", is a soft delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns the file you softDeleted</returns>
        [HttpDelete]
        [Route("remove/{id:Guid}")]
        public async Task<IActionResult> Remove([FromRoute] Guid id)
        {
            // Obtener el ID del usuario autenticado desde el token
            var authenticatedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authenticatedUserId == null || !Guid.TryParse(authenticatedUserId, out Guid authenticatedUserGuid))
            {
                return Unauthorized("User ID not found or invalid.");
            }

            var fileCheck = await fileRepository.GetByIdAsync(id);
            // Verificar que el usuario autenticado coincide con el ID solicitado
            if (authenticatedUserGuid != fileCheck.Owner.Id)
            {
                return Forbid("You are not allowed to update another user's data.");
            }

            var file = await fileRepository.SoftDelete(id);

            if (file == null)
            {
                return NotFound();
            }


            //AuditLog
            var auditLog = mapper.Map<AuditLog>(file);

            auditLog.Action = ActionType.SoftDelete;

            await auditLogRepository.CreateAuditLogAsync(auditLog);

            return Ok("Successfully removed");
        }

        /// <summary>
        /// Restore the removed file (changes the boolean IsDeleted to false)
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The restored file</returns>
        [HttpPatch]
        [Route("restore/{id:guid}")]
        public async Task<IActionResult> Restore([FromRoute] Guid id)
        {
            // Obtener el ID del usuario autenticado desde el token
            var authenticatedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authenticatedUserId == null || !Guid.TryParse(authenticatedUserId, out Guid authenticatedUserGuid))
            {
                return Unauthorized("User ID not found or invalid.");
            }

            var fileCheck = await fileRepository.GetByIdAsync(id);
            // Verificar que el usuario autenticado coincide con el ID solicitado
            if (authenticatedUserGuid != fileCheck.Owner.Id)
            {
                return Forbid("You are not allowed to update another user's data.");
            }

            var file = await fileRepository.Restore(id);

            if (file == null)
            {
                return NotFound();
            }

            //AuditLog
            var auditLog = mapper.Map<AuditLog>(file);

            auditLog.Action = ActionType.Restore;

            await auditLogRepository.CreateAuditLogAsync(auditLog);

            return Ok("Successfully restored");
        }

        /// <summary>
        /// Delete the file and the file in the folder, HARD DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            // Obtener el ID del usuario autenticado desde el token
            var authenticatedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authenticatedUserId == null || !Guid.TryParse(authenticatedUserId, out Guid authenticatedUserGuid))
            {
                return Unauthorized("User ID not found or invalid.");
            }

            var fileCheck = await fileRepository.GetByIdAsync(id);
            // Verificar que el usuario autenticado coincide con el ID solicitado
            if (authenticatedUserGuid != fileCheck.Owner.Id)
            {
                return Forbid("You are not allowed to update another user's data.");
            }

            var file = await fileRepository.HardDelete(id);

            if (file == null)
            {
                return NotFound();
            }

            //AuditLog
            var auditLog = mapper.Map<AuditLog>(file);

            auditLog.Action = ActionType.HardDelete;

            await auditLogRepository.CreateAuditLogAsync(auditLog);

            return Ok("Successfully deleted");
        } 
    }
}
