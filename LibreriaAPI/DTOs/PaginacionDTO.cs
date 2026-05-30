namespace LibreriaAPI.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;
        public int TamañoPagina { get; set; } = 10;
        public string? Busqueda { get; set; }

    }
}
