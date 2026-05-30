using LibreriaAPI.Models;

namespace LibreriaAPI.DTOs
{
    public class RespuestaPaginadaDTO
    {
        public int Pagina { get; set; }
        public int TamañoPagina { get; set; } = 10;
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
        public List<Libro> Datos { get; set; }

    }
}
