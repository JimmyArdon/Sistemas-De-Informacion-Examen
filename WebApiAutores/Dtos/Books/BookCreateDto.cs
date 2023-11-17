using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Dtos.Books
{
    public class BookCreateDto
    {
        [Display(Name = "ISBN")]
        [StringLength(50, ErrorMessage = "El {0} permite un maximo de {1} caracteres")]
        [Required(ErrorMessage = "El {0} es requerido")]
        public string ISBN { get; set; }

        [Display(Name = "Titulo")]
        [StringLength(50, ErrorMessage = "El {0} permite un maximo de {1} caracteres")]
        [Required(ErrorMessage = "El {0} es requerido")]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }

        [Display(Name = "Autor")]
        [Required(ErrorMessage = "El {0} es requerido")]
        public int AutorId { get; set; }
    }
}
