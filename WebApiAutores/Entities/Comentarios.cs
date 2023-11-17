using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApiAutores.Entities
{
    [Table("comentarios", Schema = "transacctional")]
    public class Comentarios
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("review_id")]
        public int ReviewId { get; set; }

        [Column("user_id")]
        public string User_Id { get; set; }

        [Column("descripcion")]
        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; }

        [Column("comentario_id")]
        public int? ComentarioPadre_Id { get; set; }

        [ForeignKey("User_Id")]
        public IdentityUser User { get; set; }

        [ForeignKey("ComentarioPadre_Id")]
        public Comentarios ComentarioPadre { get; set; }

        [ForeignKey("ReviewId")]
        public Reviews Review { get; set; }

        public ICollection<Comentarios> Respuestas { get; set; }
    }
}
