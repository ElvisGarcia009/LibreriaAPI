using LibreriaAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibreriaAPI.Services
{
    public class JwtTokenGenerator()
    {
        public static string GenerarTokenJWT(Usuario usuarioValidado, IConfiguration _configuracion)
        {
            // Lee la clave secreta del appsettings.json y la convierte en una clave criptográfica
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracion["Jwt:Key"]!));

            // Define el algoritmo con el que se va a firmar el token (HmacSha256 es el estándar)
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Los claims son datos del usuario que se guardan dentro del token
            // Con esto no se necesita consultar la BD en futuras peticiones para saber quien es el usuario
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, usuarioValidado.Email),
                new Claim(ClaimTypes.NameIdentifier, usuarioValidado.Id.ToString())
            };

            // Construye el token con toda la configuración
            var token = new JwtSecurityToken(
                issuer: _configuracion["Jwt:Issuer"],        // Quién emite el token (la API)
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
