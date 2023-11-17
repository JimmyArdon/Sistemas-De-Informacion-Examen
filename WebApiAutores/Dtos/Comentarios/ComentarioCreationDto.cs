using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Dtos.Comentarios
{
    public class ComentarioCreationDto
    {

        [Display(Name = "Id Review")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int Review_Id { get; set; }
        public string User_Id { get; set; }

        [Display(Name = "Comentario")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(500, ErrorMessage = "El {0} permite un máximo de {1} caracteres.")]
        public string Descripcion { get; set; }

        public int? ComentarioPadre_Id { get; set; }
    }
}
