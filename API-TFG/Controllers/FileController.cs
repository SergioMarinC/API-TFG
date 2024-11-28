using API_TFG.Data;
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

        //REMOVE FILE FROM (BOOL FALSE)
        [HttpDelete]
        [Route("remove/{id:Guid}")]
        public async Task<IActionResult> Remove([FromRoute] Guid id)
        {
            var file = await fileRepository.RemoveAsync(id);

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
            var file = await fileRepository.DeleteAsync(id);

            if (file == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<FileDto>(file));
        }
    }
}
