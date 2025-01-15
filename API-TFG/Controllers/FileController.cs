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
    public class FileController : ControllerBase
    {
        private readonly IFileRepository fileRepository;
        private readonly IAuditLogRepository auditLogRepository;
        private readonly IMapper mapper;

        public FileController(IFileRepository fileRepository, IAuditLogRepository auditLogRepository, IMapper mapper)
        {
            this.fileRepository = fileRepository;
            this.auditLogRepository = auditLogRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// Get all files in the database
        /// </summary>
        /// <returns>List of Files</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var files = await fileRepository.GetAllAsync();

            return Ok(mapper.Map<List<FileDto>>(files));
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
            var file = await fileRepository.GetByIdAsync(id);

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
        public async Task<IActionResult> GetByUserId([FromRoute] Guid id)
        {
            var files = await fileRepository.GetAllByUserIdAsync(id);
            
            if(files == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<FileDto>>(files));
        }

        /// <summary>
        /// Creates a file and uploads the real file to the folder
        /// </summary>
        /// <param name="fileUploadDto"></param>
        /// <returns>File</returns>
        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadDto fileUploadDto)
        {
            if (ModelState.IsValid)
            {
                var file = mapper.Map<Models.Domain.File>(fileUploadDto);

                var savedFile = await fileRepository.UploadAsync(file, fileUploadDto.UploadedFile);

                //AuditLog
                var auditLog = mapper.Map<AuditLog>(savedFile);
                
                auditLog.Action = Models.Enum.ActionType.Upload;
                
                await auditLogRepository.CreateAuditLogAsync(auditLog);

                return Ok(mapper.Map<FileDto>(savedFile));
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Updates the name and the folder path of a existing File, it changes the place of it in the folder
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateFileRequestDto"></param>
        /// <returns>The updated File</returns>
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateFileRequestDto updateFileRequestDto)
        {
            if (ModelState.IsValid)
            {
                var updatedFile = mapper.Map<Models.Domain.File>(updateFileRequestDto);

                var file = await fileRepository.UpdateAsync(id, updatedFile);

                if (file == null)
                {
                    return NotFound();
                }

                //AuditLog
                var auditLog = mapper.Map<AuditLog>(file);

                auditLog.Action = Models.Enum.ActionType.Update;

                await auditLogRepository.CreateAuditLogAsync(auditLog);

                return Ok(mapper.Map<FileDto>(file));
            }
            else
            {
                return BadRequest(ModelState);
            }
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

            //AuditLog
            var auditLog = mapper.Map<AuditLog>(file);

            auditLog.Action = Models.Enum.ActionType.Download;

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
            var file = await fileRepository.SoftDelete(id);

            if(file == null)
            {
                return NotFound();
            }

            //AuditLog
            var auditLog = mapper.Map<AuditLog>(file);

            auditLog.Action = Models.Enum.ActionType.SoftDelete;

            await auditLogRepository.CreateAuditLogAsync(auditLog);

            return Ok(mapper.Map<FileDto>(file));
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
            var file = await fileRepository.Restore(id);

            if (file == null)
            {
                return NotFound();
            }

            //AuditLog
            var auditLog = mapper.Map<AuditLog>(file);

            auditLog.Action = Models.Enum.ActionType.Restore;

            await auditLogRepository.CreateAuditLogAsync(auditLog);

            return Ok(mapper.Map<FileDto>(file));
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
            var file = await fileRepository.HardDelete(id);

            if (file == null)
            {
                return NotFound();
            }

            //AuditLog
            var auditLog = mapper.Map<AuditLog>(file);

            auditLog.Action = Models.Enum.ActionType.HardDelete;

            await auditLogRepository.CreateAuditLogAsync(auditLog);

            return Ok(mapper.Map<FileDto>(file));
        }
    }
}
