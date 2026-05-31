using LibreriaAPI.DB;
using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using LibreriaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LibreriaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(LibreriaContext context, IConfiguration configuracion) : ControllerBase
    {
        private readonly LibreriaContext _context = context;
        private readonly IConfiguration _configuracion = configuracion;

        [HttpGet("{id}")]
        public ActionResult<usuarioDTO> ObtenerUsuarioPorID(int id)
        {
            if(_context.usuarios.Any(usuario => usuario.Id == id))
            {
                var EncontrarUsuario = _context.usuarios.FirstOrDefault(user => user.Id == id);

                usuarioDTO usuarioEncontrado = new()
                {
                    Email = EncontrarUsuario!.Email,
                    Id = EncontrarUsuario.Id
                };

                return Ok(usuarioEncontrado);
            }
            else
            {
                return NotFound();
            }
        }



        [HttpPost("/registro")]
        public ActionResult<usuarioDTO> Registro(RegistroDTO nuevoUsuario)
        {
            if(_context.usuarios.Any(usuario => usuario.Email == nuevoUsuario.Email))
            {
                return BadRequest();
            }

            Usuario UsuarioValidado = new()
            {
                Email = nuevoUsuario.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevoUsuario.Password)
            };

            _context.Add(UsuarioValidado);
            _context.SaveChanges();

            usuarioDTO usuarioGuardado = new()
            {
                Id = UsuarioValidado.Id,
                Email = UsuarioValidado.Email
            };

            return CreatedAtAction(nameof(ObtenerUsuarioPorID), new { id = usuarioGuardado.Id }, usuarioGuardado);
        }



        [HttpPost("/login")]
        public ActionResult<string> Login(LoginDTO usuario)
        {
            if(!_context.usuarios.Any(user => user.Email == usuario.Email))
            {
                return Unauthorized();
            }
                
            Usuario? ValidarUsuario = _context.usuarios.FirstOrDefault(user => user.Email == usuario.Email);

            if (!BCrypt.Net.BCrypt.Verify(usuario.Password, ValidarUsuario!.PasswordHash))
            {
                return Unauthorized();
            }

            string TokenGenerado = JwtTokenGenerator.GenerarTokenJWT(ValidarUsuario, _configuracion);

            return Ok(TokenGenerado);

        }
    }
}
