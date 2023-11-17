using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebApiAutores.Dtos.Comentarios; 
using WebApiAutores.Entities;
using WebApiAutores.Services;

namespace WebApiAutores.Controllers
{
    [Route("api/comentarios")]
    [ApiController]
    [Authorize]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebPurifyService _webPurifyService;

        public ComentariosController(ApplicationDbContext dbContext,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IWebPurifyService webPurifyService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _webPurifyService = webPurifyService;
        }

        [HttpPost]
        public async Task<ActionResult> AgregarComentario([FromBody] ComentarioCreationDto comentarioDto)
        {
            var idClaim = User.Claims.Where(x => x.Type == "UserId").FirstOrDefault();
            var userId = idClaim.Value;

            if (userId == null)
            {
                return BadRequest("No se pudo obtener el Id del usuario.");
            }

            
            var reviewExistente = await _dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == comentarioDto.Review_Id);

            if (reviewExistente == null)
            {
                return NotFound($"La review con Id {comentarioDto.Review_Id} no existe.");
            }

            // Verificar la toxicidad del comentario utilizando WebPurify
            var isProfane = await _webPurifyService.CheckForProfanity(comentarioDto.Descripcion);

            
            try
            {
                // Crea un objeto XElement a partir del XML
                XElement xmlElement = XElement.Parse(isProfane);

                // Obtén el valor de <found> como un entero
                int foundValue = (int)xmlElement.Element("found");

                // Realiza la comparación
                if (foundValue > 0)
                {
                    return BadRequest("La descripción contiene palabras ofensivas.");
                }
            }
            catch (Exception ex)
            {
               
                return BadRequest("Error al procesar la respuesta del servicio de WebPurify. Detalles: " + ex.Message);
            }

            var comentario = new Comentarios
            {
                ReviewId = comentarioDto.Review_Id,
                User_Id = userId,
                Descripcion = comentarioDto.Descripcion,
                ComentarioPadre_Id = comentarioDto.ComentarioPadre_Id 
            };

            if (comentarioDto.ComentarioPadre_Id.HasValue && comentarioDto.ComentarioPadre_Id.Value > 0)
            {
                var comentarioPadre = await _dbContext.Comentarios.FindAsync(comentarioDto.ComentarioPadre_Id.Value);
                if (comentarioPadre != null)
                {
                    comentario.ComentarioPadre = comentarioPadre;
                }
                else
                {
                    return NotFound($"El comentario padre con Id {comentarioDto.ComentarioPadre_Id.Value} no existe.");
                }
            }
            else
            {
                comentario.ComentarioPadre_Id = null; 
            }

            _dbContext.Comentarios.Add(comentario);
            await _dbContext.SaveChangesAsync();

            return Ok("Comentario agregado exitosamente.");
        }

        [HttpGet("{reviewId}")]
        [AllowAnonymous]
        public IActionResult ObtenerComentariosPorReview(int reviewId)
        {
            try
            {
                // Verificar si la revisión existe
                var existingReview = _dbContext.Reviews.FirstOrDefault(r => r.Id == reviewId);
                if (existingReview == null)
                {
                    return NotFound("La revisión no existe.");
                }

                // Obtener todos los comentarios asociados a la revisión
                var comentarios = _dbContext.Comentarios.Where(c => c.ReviewId == reviewId).ToList();
                var comentariosDto = _mapper.Map<List<ComentarioDto>>(comentarios);

                return Ok(comentariosDto);
            }
            catch (Exception ex)
            {
                // Loguear la excepción o manejarla de otra manera según tus necesidades
                Console.WriteLine($"Error al obtener comentarios: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener comentarios.");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> EditarReview([FromBody] ComentariosUpdateDtos comentarioDto, int id)
        {
            try
            {
                // Verificar si el comentario existe
                var existingComentario = await _dbContext.Comentarios.FirstOrDefaultAsync(c => c.Id == id);
                if (existingComentario == null)
                {
                    return NotFound("El comentario no existe.");
                }

                // Verificar si el usuario actual es el propietario del comentario
                var idClaim = User.Claims.Where(x => x.Type == "UserId").FirstOrDefault();
                var userId = idClaim?.Value;

                if (userId != existingComentario.User_Id)
                {
                    return Forbid("No tienes permisos para editar este comentario.");
                }

                // Verificar si el review asociado existe
                var existingReview = await _dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == comentarioDto.ReviewId);
                if (existingReview == null)
                {
                    return NotFound("La revisión asociada no existe.");
                }

                // Mapear propiedades del DTO a la entidad del comentario usando AutoMapper
                _mapper.Map(comentarioDto, existingComentario);
                comentarioDto.User_Id = userId;

                // Guardar los cambios en la base de datos
                await _dbContext.SaveChangesAsync();

                return Ok("Comentario editado exitosamente.");
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error al editar el comentario: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al editar el comentario.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarComentario(int id)
        {
            try
            {
                // Verificar si el comentario existe
                var existingComentario = await _dbContext.Comentarios.FirstOrDefaultAsync(c => c.Id == id);
                if (existingComentario == null)
                {
                    return NotFound("El comentario no existe.");
                }

                // Verificar si el usuario actual es el propietario del comentario
                var idClaim = User.Claims.Where(x => x.Type == "UserId").FirstOrDefault();
                var userId = idClaim?.Value;

                if (userId != existingComentario.User_Id)
                {
                    return Forbid("No tienes permisos para eliminar este comentario.");
                }

                // Eliminar el comentario de la base de datos
                _dbContext.Comentarios.Remove(existingComentario);
                await _dbContext.SaveChangesAsync();

                return Ok("Comentario eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                // Loguear la excepción o manejarla de otra manera según tus necesidades
                Console.WriteLine($"Error al eliminar el comentario: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al eliminar el comentario.");
            }
        }

    }
}
