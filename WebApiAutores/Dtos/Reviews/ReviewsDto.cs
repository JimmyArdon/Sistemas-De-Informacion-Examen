using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Entities;

namespace WebApiAutores.Dtos.Reviews
{
    public class ReviewsDto
    {
        public int Id { get; set; }
        public string User_Id { get; set; }
        public Guid Book_Id { get; set; }
        public string Descripcion { get; set; }
        public int Calificacion { get; set; }
    }
}
