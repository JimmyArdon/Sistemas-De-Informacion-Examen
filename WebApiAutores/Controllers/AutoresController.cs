using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Dtos;
using WebApiAutores.Dtos.Autores;
using WebApiAutores.Entities;

namespace WebApiAutores.Controllers
{
    [Route("api/autores")]
    [ApiController]
    //[Authorize]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AutoresController(ApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<IReadOnlyList<AutorDto>>>> Get() 
        {
            var autoresDb = await _context.Autores.ToListAsync();
            var autoresDto = _mapper.Map<List<AutorDto>>(autoresDb);
            return Ok(new ResponseDto<List<AutorDto>>
            {
                Status = true,
                Data = autoresDto
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDto<AutorGetByIdDto>>> GetOneById(int id) 
        {
            var autorDb = await _context.Autores
                .Include(a => a.Books)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (autorDb is null)
            {
                return NotFound(new ResponseDto<AutorGetByIdDto> 
                {
                    Status = false,
                    Message = $"El autor con id {id}, no fue encontrado."
                });
            }

            var autorDto = _mapper.Map<AutorGetByIdDto>(autorDb);
            
            return Ok(new ResponseDto<AutorGetByIdDto> 
            {
                Status = true,
                Data = autorDto
            });
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<AutorDto>>> Post(AutorCreateDto dto) 
        {
            var autor = _mapper.Map<Autor>(dto);

            _context.Add(autor);
            await _context.SaveChangesAsync();

            var autorDto = _mapper.Map<AutorDto>(autor);

            return StatusCode(StatusCodes.Status201Created, new ResponseDto<AutorDto> 
            {
                Status = true,
                Data = autorDto
            });
        }

        [HttpPut("{id:int}")] // api/autores/4
        public async Task<ActionResult<ResponseDto<AutorGetByIdDto>>> Put(int id, AutorUpdateDto dto) 
        {
            var autorDb = await _context.Autores.FirstOrDefaultAsync(a => a.Id == id);
            if (autorDb is null)
            {
                return NotFound(new ResponseDto<AutorGetByIdDto>
                {
                    Status = false,
                    Message = $"El autor con id {id}, no fue encontrado."
                });
            }

            _mapper.Map<AutorUpdateDto, Autor>(dto, autorDb);

            _context.Update(autorDb);
            await _context.SaveChangesAsync();

            var autorDto = _mapper.Map<AutorGetByIdDto>(autorDb);

            return Ok(new ResponseDto<AutorGetByIdDto> 
            {
                Status = true,
                Message = "Autor editado correctamente",
                Data = autorDto
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ResponseDto<string>>> Delete(int id) 
        {
            var autor = await _context.Autores.FirstOrDefaultAsync(a => a.Id == id);
            if (autor is null)
            {
                return NotFound(new ResponseDto<AutorGetByIdDto>
                {
                    Status = false,
                    Message = $"El autor con id {id}, no fue encontrado."
                });
            }

            _context.Remove(autor);
            await _context.SaveChangesAsync();

            return Ok(new ResponseDto<string> 
            {
                Status = true,
                Message = $"El autor con el id {id} fue borrado"
            });
        }
    }
}
