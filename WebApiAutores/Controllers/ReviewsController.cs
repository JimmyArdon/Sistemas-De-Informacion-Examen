
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebApiAutores.Dtos;
using WebApiAutores.Dtos.Reviews;
using WebApiAutores.Entities;
using WebApiAutores.Services;

namespace WebApiAutores.Controllers
    {
        [Route("api/reviews")]
        [ApiController]
        [Authorize]
        public class ReviewsController : ControllerBase
        {
            private readonly ApplicationDbContext _dbContext;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IWebPurifyService _webPurifyService;
            private readonly IConfiguration _configuration;

            public ReviewsController(ApplicationDbContext dbContext, 
                IMapper mapper,
                IHttpContextAccessor httpContextAccessor,
                IWebPurifyService webPurifyService,
                IConfiguration configuration)
            {
                _dbContext = dbContext;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
                _webPurifyService = webPurifyService;
                _configuration = configuration;
            }

            [HttpPost]
        
        public async Task<IActionResult> CrearReview([FromBody] ReviewsCreationDto reviewDto)
            {
                try
                {
                    var idClaim = User.Claims.Where(x => x.Type == "UserId").FirstOrDefault();
                    var userId = idClaim?.Value;

                    if (userId == null)
                    {
                        return BadRequest("No se pudo obtener el Id del usuario.");
                    }

                   
                    if (!_dbContext.Books.Any(l => l.Id == reviewDto.Book_Id))
                    {
                        return NotFound("El libro no existe.");
                    }

                    // Verificar la toxicidad del comentario utilizando WebPurify
                    var isProfane = await _webPurifyService.CheckForProfanity(reviewDto.Descripcion);

                    
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

                    
                var review = _mapper.Map<Reviews>(reviewDto);
                review.User_Id = userId;

                _dbContext.Reviews.Add(review);
                    await _dbContext.SaveChangesAsync();

                await ActualizarValoracionPromedio(reviewDto.Book_Id);

                return Ok("Review creada exitosamente.");
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error al crear la revisión.");
                }
            }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<IReadOnlyList<ReviewsDto>>>> ObtenerReviews()
        {
            try
            {
                var reviewsDb = await _dbContext.Reviews
                    .Include(r => r.Comentarios)
                    .ToListAsync();

                var reviewsDto = _mapper.Map<List<ReviewsDto>>(reviewsDb);

                return new ResponseDto<IReadOnlyList<ReviewsDto>>
                {
                    Status = true,
                    Data = reviewsDto
                };
            }
            catch (Exception ex)
            {
                // Loguear la excepción o manejarla de otra manera según tus necesidades
                Console.WriteLine($"Error al obtener revisiones: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener revisiones.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarReview([FromBody] ReviewsUpdateDto reviewDto, int id)
        {
            try
            {
                // Verificar si la revisión existe
                var existingReview = await _dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == id);
                if (existingReview == null)
                {
                    return NotFound("La revisión no existe.");
                }

                // Verificar si el usuario actual es el propietario de la revisión
                var idClaim = User.Claims.Where(x => x.Type == "UserId").FirstOrDefault();
                var userId = idClaim?.Value;

                if (userId != existingReview.User_Id)
                {
                    return Forbid("No tienes permisos para editar esta revisión.");
                }

                // Verificar si el libro asociado existe
                if (!_dbContext.Books.Any(l => l.Id == reviewDto.Book_Id))
                {
                    return NotFound("El libro no existe.");
                }

                // Mapear propiedades del DTO a la entidad de revisión usando AutoMapper
                _mapper.Map(reviewDto, existingReview);
                reviewDto.User_Id = userId;

                await ActualizarValoracionPromedio(reviewDto.Book_Id);

                // Guardar los cambios en la base de datos
                await _dbContext.SaveChangesAsync();

                return Ok("Review editada exitosamente.");
            }
            catch (Exception ex)
            {
             
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al editar la revisión.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarReview(int id)
        {
            try
            {
                // Verificar si la revisión existe
                var existingReview = await _dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == id);
                if (existingReview == null)
                {
                    return NotFound("La revisión no existe.");
                }

                // Verificar si el usuario actual es el propietario de la revisión
                var idClaim = User.Claims.Where(x => x.Type == "UserId").FirstOrDefault();
                var userId = idClaim?.Value;

                if (userId != existingReview.User_Id)
                {
                    return Forbid("No tienes permisos para eliminar esta revisión.");
                }

                // Eliminar la revisión de la base de datos
                _dbContext.Reviews.Remove(existingReview);
                await _dbContext.SaveChangesAsync();

                return Ok("Revisión eliminada exitosamente.");
            }
            catch (Exception ex)
            {
              
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al eliminar la revisión.");
            }
        }

        private async Task ActualizarValoracionPromedio(Guid bookId)
        {
            var book = await _dbContext.Books
                .Include(b => b.Reviews)  // Asegúrate de incluir las revisiones
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book != null)
            {
                // Calcula el promedio de las calificaciones
                decimal valoracionPromedio = (decimal)(book.Reviews.Any()
                    ? book.Reviews.Average(r => r.Calificacion)
                    : 0);

                // Actualiza la propiedad ValoracionPromedio
                book.ValoracionPromedio = valoracionPromedio;

                // Guarda los cambios en la base de datos
                await _dbContext.SaveChangesAsync();
            }
        }

    }


}
