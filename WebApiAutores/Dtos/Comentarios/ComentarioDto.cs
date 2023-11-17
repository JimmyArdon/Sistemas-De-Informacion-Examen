using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Dtos.Comentarios
{
    public class ComentarioDto
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string User_Id { get; set; }
        public string Descripcion { get; set; }
        public int? ComentarioPadre_Id { get; set; }
    }
}
