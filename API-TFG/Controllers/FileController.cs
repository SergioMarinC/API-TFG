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
        private readonly AppDbContext dbContext;
        private readonly IFileRepository fileRepository;
        private readonly IMapper mapper;

        public FileController(AppDbContext dbContext, IFileRepository fileRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.fileRepository = fileRepository;
            this.mapper = mapper;
        }

        //GET ALL FILES
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var files = await fileRepository.GetAllAsync();

            return Ok(mapper.Map<List<FileDto>>(files));
        }

        //GET BY ID
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

        //GET ALL BY USER ID
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

        //UPLOAD A FILE
        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadDto fileUploadDto)
        {
            if (ModelState.IsValid)
            {
                var file = mapper.Map<Models.Domain.File>(fileUploadDto);

                var savedFile = await fileRepository.UploadAsync(file, fileUploadDto.UploadedFile);

                return Ok(mapper.Map<FileDto>(savedFile));
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        //UPDATE FILE
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

                return Ok(mapper.Map<FileDto>(file));
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

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

            // Determinar el tipo MIME
            var contentType = fileRepository.GetContentType(file.FilePath);

            // Devolver el archivo como respuesta
            return File(fileContent, contentType, file.FileName);
        }


        //REMOVE FILE FROM (BOOL FALSE)
        [HttpDelete]
        [Route("remove/{id:Guid}")]
        public async Task<IActionResult> Remove([FromRoute] Guid id)
        {
            var file = await fileRepository.SoftDelete(id);

            if(file == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<FileDto>(file));
        }

        //DELETE FILE FROM DATABASE
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var file = await fileRepository.HardDelete(id);

            if (file == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<FileDto>(file));
        }
    }
}
