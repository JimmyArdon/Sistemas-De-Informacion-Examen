using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApiAutores.Entities
{
    [Table("reviews", Schema = "transacctional")]
    public class Reviews
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("user_id")]
        public string User_Id { get; set; }

        [Column("book_id")]
        public Guid Book_Id { get; set; }

        [Column("descripcion")]
        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; }

        [Column("calificacion")]
        [Range(1, 5)] 
        public int Calificacion { get; set; }
        public ICollection<Comentarios> Comentarios { get; set; }

        [ForeignKey("User_Id")]
        public IdentityUser User { get; set; }

        [ForeignKey("Book_Id")]
        public Book Book { get; set; }
    }
}
