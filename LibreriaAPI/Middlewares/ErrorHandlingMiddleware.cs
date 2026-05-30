using System.Net;
using System.Text.Json;

namespace LibreriaAPI.Middleware
{
    public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        //Todos los Middlewares llevan el next
        private readonly RequestDelegate _next = next;

        //Inyectamos una instancia del logger para acceder a funciones como _Logger.Information, _logger.LogInformation,LogError o LogWarning
        //Recordar que esta instancia es porque este middleware es un errorhandling, no es obligatorio para todos los middlewares
        private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

        //Con esta funcion, ASP.NET Core sabe que esta clase es un middleware y lo llama siempre en cada peticion que se haga a la API
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            } 
            catch(Exception e)
            {
                _logger.LogError(e, e.Message);
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var result = new { status = 500, mensaje = "Error interno del servidor" };
                var Json = JsonSerializer.Serialize(result);

                await context.Response.WriteAsync(Json);

            }
        }
    }
}
