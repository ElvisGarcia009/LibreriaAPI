using LibreriaAPI.DB;
using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

            string TokenGenerado = GenerarTokenJWT(ValidarUsuario);

            return Ok(TokenGenerado);

        }


        private string GenerarTokenJWT(Usuario usuarioValidado)
        {
            // Lee la clave secreta del appsettings.json y la convierte en una clave criptográfica
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracion["Jwt:Key"]!));

            // Define el algoritmo con el que se va a firmar el token (HmacSha256 es el estándar)
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Los claims son datos del usuario que se guardan dentro del token
            // No necesitas consultar la BD en futuras peticiones para saber quién es el usuario
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, usuarioValidado.Email),
                new Claim(ClaimTypes.NameIdentifier, usuarioValidado.Id.ToString())
            };

            // Construye el token con toda la configuración
            var token = new JwtSecurityToken(
                issuer: _configuracion["Jwt:Issuer"],        // Quién emite el token (tu API)
                audience: _configuracion["Jwt:Audience"],    // Quién puede usar el token
                claims: claims,                              // Datos del usuario dentro del token
                expires: DateTime.UtcNow.AddHours(8),        // El token expira en 8 horas
                signingCredentials: credenciales             // Firma del token
            );

            // Convierte el token a string y lo retorna
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
