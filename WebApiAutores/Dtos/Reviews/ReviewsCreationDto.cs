using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Dtos.Reviews
{
    public class ReviewsCreationDto
    {

        public string User_Id { get; set; }

        [Display(Name = "Id de Libro")]
        [Required(ErrorMessage = "El {0} es requerido")]
        public Guid Book_Id { get; set; }

        [Display(Name = "Comentario")]
        [StringLength(500, ErrorMessage = "El {0} permite un máximo de {1} caracteres")]
        [Required(ErrorMessage = "El {0} es requerido")]
        [RegularExpression(@"^[^\d]+$", ErrorMessage = "La descripción no debe contener números")]

        public string Descripcion { get; set; }

        [Display(Name = "Calificacion")]
        [Range(1, 5, ErrorMessage = "La {0} debe estar en el rango de {1} a {2}")]
        [Required(ErrorMessage = "El {0} es requerido")]
        public int Calificacion { get; set; }

    }
}
