namespace WebApiAutores.Dtos.Reviews
{
    public class ReviewsUpdateDto
    {
        public Guid Book_Id { get; set; }

        public string User_Id { get; set; }
        public string Descripcion { get; set; }
        public int Calificacion { get; set; }
    }
}
