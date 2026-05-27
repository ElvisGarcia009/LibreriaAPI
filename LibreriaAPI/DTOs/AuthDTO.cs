using System.ComponentModel.DataAnnotations;

namespace LibreriaAPI.DTOs
{

    public class usuarioDTO
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";

    }
    public class RegistroDTO
    {

        [Required(ErrorMessage = "Debe colocar un Email válido")]
        [EmailAddress]
        [StringLength(50, ErrorMessage = "El correo no puede tener más de 50 caracteres")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "El campo Password es obligatorio")]
        [StringLength(100, ErrorMessage = "Password no puede tener mas de 100 caracteres")]
        public required string Password { get; set; }
    }

    public class LoginDTO
    {
        
        [Required(ErrorMessage = "Debe colocar un Email válido")]
        [EmailAddress]
        [StringLength(50, ErrorMessage = "El correo no puede tener más de 50 caracteres")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "El campo Password es obligatorio")]
        [StringLength(100, ErrorMessage = "Password no puede tener mas de 100 caracteres")]
        public required string Password { get; set; }
    }
}
