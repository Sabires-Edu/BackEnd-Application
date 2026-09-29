using Microsoft.AspNetCore.Diagnostics;
using Sabires_Backend_Application.Models.Response;

namespace Sabires_Backend_Application.Utils.Exceptions
{
    public class CustomerExceptionsHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
                HttpContext httpContext,
                Exception exception,
                CancellationToken cancellationToken
            )
        {
            var depara = new Dictionary<Type, (int Status, string Message)>
            {
                [typeof(BadRequestException)] = (400, "Houve um erro ao criar o usuário")
            };

            if (depara.TryGetValue(exception.GetType(), out var value))
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                ErrorResponse errorResponse = new()
                {
                    Success = false,
                    Title = value.Message,
                    Detail = exception.Message,
                    Status = value.Status
                };

                await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
                return true;
            }

            return false;
        }
    }
}
