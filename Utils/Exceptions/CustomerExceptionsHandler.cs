using Microsoft.AspNetCore.Diagnostics;
using Sabires_Backend_Application.Models.Response;

namespace Sabires_Backend_Application.Utils.Exceptions
{
    public class CustomerExceptionsHandler : IExceptionHandler
    {
        public static readonly Dictionary<Type, (int Status, string Message)> depara = new()        
        {
            { typeof(BadRequestException), (400, "Houve um erro ao realizar a requisição") },
            { typeof(InternalServerException), (500, "Houve um erro interno durante a requisição") },
        };

        public async ValueTask<bool> TryHandleAsync(
                HttpContext httpContext,
                Exception exception,
                CancellationToken cancellationToken
            )
        {
            var entry = depara.FirstOrDefault(ex => ex.Key.IsInstanceOfType(exception));

            if (entry.Key is null)
                return false;

            var (status, title) = entry.Value;

            httpContext.Response.StatusCode = status;

            ErrorResponse errorResponse = new()
            {
                Success = false,
                Title = title,
                Detail = exception.Message,
                Status = status
            };

            await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
            return true;
        }
    }
}
