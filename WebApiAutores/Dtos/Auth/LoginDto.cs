using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Dtos.Auth
{
    public class LoginDto
    {
        [Display(Name = "Correo Electrónico")]
        [Required(ErrorMessage = "El {0} es requerido")]
        public string Email { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "La {0} es requerida")]
        public string Password { get; set; }
    }
}
