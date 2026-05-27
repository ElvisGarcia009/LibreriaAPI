using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibreriaAPI.Models
{
    public class Libro
    {
        [Key]
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public string Autor { get; set; } = "";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

       /** public static bool EncontrarLibro(List<Libro> libros, int id)
        {
            if (libros.Any(n => n.Id == id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }**/

    }
}
