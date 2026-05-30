using LibreriaAPI.DB;
using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LibrosController : ControllerBase
    {
        private readonly LibreriaContext _context;

        public LibrosController(LibreriaContext context)
        {
            _context = context;
        }

        //BUSCAR TODOS LOS LIBROS
        //[FromQuery] le dice a ASP.NET que los parámetros vienen en la URL como query strings, no en el body
        [HttpGet]
        public ActionResult<RespuestaPaginadaDTO> ObtenerLibros([FromQuery] PaginacionDTO paginacion)

        {
            var result = _context.libros.Select(libro => new LibroDTO
            {

                Id = libro.Id,
                Titulo = libro.Titulo,
                Autor = libro.Autor,
                Precio = libro.Precio

            }).ToList();

            return Ok(result);
        }



        //ENCONTRAR LIBRO POR ID (IDS UNICOS -> UN SOLO LIBRO)
        [HttpGet("{id}")]
        public ActionResult<LibroDTO> EncontrarLibroPorId(int id)
        {

            Libro? result = _context.libros.FirstOrDefault(libro => libro.Id == id);

            if(result != null)
            {
                LibroDTO LibroEncontrado = new LibroDTO
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
            Libro nuevoLibro = new Libro
            {
                Titulo = libro.Titulo,
                Autor = libro.Autor,
                Precio = libro.Precio
            };

            _context.Add(nuevoLibro);
            _context.SaveChanges();

            LibroDTO libroGuardado = new LibroDTO
            {
                Id = nuevoLibro.Id,
                Titulo = nuevoLibro.Titulo,
                Autor = nuevoLibro.Autor,
                Precio = nuevoLibro.Precio
            };

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

                return NoContent();
            } 
            else
            {
                return NotFound();
            }
               
        }
    }
}
