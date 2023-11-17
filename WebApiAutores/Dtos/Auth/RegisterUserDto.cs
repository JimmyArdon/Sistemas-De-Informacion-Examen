using Microsoft.OpenApi.Attributes;
using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Dtos.Auth
{
    public class RegisterUserDto
    {
        [Microsoft.OpenApi.Attributes.Display(nameof(Email))]
        [Required (ErrorMessage = " El {0} es requerido.")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", 
            ErrorMessage = "Ingrese un {0} valido.")]
        public string Email { get; set; }

        [Microsoft.OpenApi.Attributes.Display(nameof(Password))]
        [Required(ErrorMessage = " El {0} es requerido.")]
        public string Password { get; set; }

        [Compare(nameof(Password),ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}
