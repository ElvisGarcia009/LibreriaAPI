using LibreriaAPI.DB;
using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LibreriaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LibrosController(LibreriaContext context, IMemoryCache memoryCache) : ControllerBase
    {
        private readonly LibreriaContext _context = context;
        private readonly IMemoryCache _cache = memoryCache;
        private static int _cacheVersion = 0;


        //BUSCAR TODOS LOS LIBROS
        //[FromQuery] le dice a ASP.NET que los parámetros vienen en la URL como query strings, no en el body 
        [HttpGet]
        public ActionResult<RespuestaPaginadaDTO> ObtenerLibros([FromQuery] PaginacionDTO paginacion)

        {
            //KEY VALUE parecido al que tuve que hacer en PocketPlanner
            string cacheKey = $"libros_v{_cacheVersion}_p{paginacion.Pagina}_t{paginacion.TamañoPagina}_b{paginacion.Busqueda}";

            if (_cache.TryGetValue(cacheKey, out RespuestaPaginadaDTO? cachedResult))
            {
                return Ok(cachedResult);
            }

            var query = _context.libros.AsQueryable();

            if (!string.IsNullOrEmpty(paginacion.Busqueda))
            {
                query = query.Where(l => l.Titulo.Contains(paginacion.Busqueda) || l.Autor.Contains(paginacion.Busqueda));
            }

            int totalRegistros = query.Count();
            query = query.Skip((paginacion.Pagina - 1) * paginacion.TamañoPagina).Take(paginacion.TamañoPagina);

            List<LibroDTO> convertirAlibroDTO = query.Select(l => new LibroDTO
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Autor = l.Autor,
                Precio = l.Precio
            }).ToList();

            RespuestaPaginadaDTO result = new ()
            {
                Pagina = paginacion.Pagina,
                TamañoPagina = paginacion.TamañoPagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = (int)Math.Ceiling((double)totalRegistros / paginacion.TamañoPagina),
                Datos = convertirAlibroDTO
            };

            _cache.Set(cacheKey, result, TimeSpan.FromSeconds(30));
            return Ok(result);
        }



        //ENCONTRAR LIBRO POR ID (IDS UNICOS -> UN SOLO LIBRO)
        [HttpGet("{id}")]
        public ActionResult<LibroDTO> EncontrarLibroPorId(int id)
        {

            Libro? result = _context.libros.FirstOrDefault(libro => libro.Id == id);

            if(result != null)
            {
                LibroDTO LibroEncontrado = new()
                {
                    Id = result.Id,
                    Titulo = result.Titulo,
                    Autor = result.Autor,
                    Precio = result.Precio
                };

                return Ok(LibroEncontrado);
            }
            else
            {
                return NotFound();
            }

        }


        //CREAR UN NUEVO LIBRO
        [HttpPost]
        public ActionResult<LibroDTO> SubirLibro(CrearLibroDTO libro)
        {
            Libro nuevoLibro = new()
            {
                Titulo = libro.Titulo,
                Autor = libro.Autor,
                Precio = libro.Precio
            };

            _context.Add(nuevoLibro);
            _context.SaveChanges();

            LibroDTO libroGuardado = new ()
            {
                Id = nuevoLibro.Id,
                Titulo = nuevoLibro.Titulo,
                Autor = nuevoLibro.Autor,
                Precio = nuevoLibro.Precio
            };

            _cacheVersion++;
            return CreatedAtAction(nameof(EncontrarLibroPorId), new {id = libroGuardado.Id}, libroGuardado);

        }


        //ACTUALIZAR UN LIBRO
        [HttpPut("{id}")]
        public ActionResult<LibroDTO> ActualizarLibro(int id, CrearLibroDTO nuevoLibro)
        {
            Libro contenerLibro = new()
            {
                Titulo = nuevoLibro.Titulo,
                Autor = nuevoLibro.Autor,
                Precio = nuevoLibro.Precio
            };


            if (_context.libros.Any(libro => libro.Id == id))
            {
                contenerLibro.Id = id;

                _context.libros.Update(contenerLibro);
                _context.SaveChanges();

                LibroDTO devolverLibro = new()
                {
                    Id = contenerLibro.Id,
                    Titulo = contenerLibro.Titulo,
                    Autor = contenerLibro.Autor,
                    Precio = contenerLibro.Precio
                };

                _cacheVersion++;

                return Ok(devolverLibro);
            }
            else
            {
                return NotFound();
            }
        }


        //BORRAR UN LIBRO
        [HttpDelete("{id}")]
        public ActionResult<LibroDTO> BorrarLibro(int id)
        {
            if (_context.libros.Any(libro => libro.Id == id))
            {
                Libro libroEliminado = _context.libros.First(libro => libro.Id == id);
                _context.libros.Remove(libroEliminado);
                _context.SaveChanges();
                _cacheVersion++;

                return NoContent();
            } 
            else
            {
                return NotFound();
            }
               
        }
    }
}
