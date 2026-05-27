using LibreriaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibreriaAPI.DB
{
    public class LibreriaContext : DbContext
    {
        public DbSet<Libro> libros { get; set; }
        public DbSet<Usuario> usuarios { get; set; }

        public LibreriaContext(DbContextOptions<LibreriaContext> options) : base(options)
        {

        }
    }
}
