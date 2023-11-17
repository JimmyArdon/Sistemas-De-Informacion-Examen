using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiAutores.Entities
{
    [Table("autores", Schema = "transacctional")]
    public class Autor
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [Required]
        [StringLength(70)]
        public string Name { get; set; }

        public virtual IEnumerable<Book> Books { get; set; }
    }
}
