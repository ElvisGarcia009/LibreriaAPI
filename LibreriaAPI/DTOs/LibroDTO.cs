using System.ComponentModel.DataAnnotations;

namespace LibreriaAPI.DTOs
{
    public class LibroDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public string Autor { get; set; } = "";
        public decimal Precio { get; set; }
    }

    public class CrearLibroDTO
    {
        [Required(ErrorMessage ="El título es obligatorio")]
        [StringLength(100, ErrorMessage = "El título no puede tener más de 100 caracteres")]
        public required string Titulo { get; set; }

        [Required(ErrorMessage = "El autor es obligatorio")]
        [StringLength(100, ErrorMessage = "Autor no puede tener mas de 100 caracteres")]
        public required string Autor { get; set; }

        [Range(0.01, 10000, ErrorMessage = "El precio debe estar entre 0.01 y 10,000")]
        public decimal Precio { get; set; } = 0.01m;
    }
}
