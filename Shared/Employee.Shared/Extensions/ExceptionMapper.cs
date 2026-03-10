using Employee.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Employee.Shared.Extensions;

public class ExceptionMapper
{
    public static (int StatusCode, string Message) Map(Exception exception)
    {
        var ex = exception.GetBaseException();
        return ex switch
        {
            AppException appEx => (appEx.StatusCode, appEx.Message),

            UnauthorizedAccessException =>
                (StatusCodes.Status401Unauthorized, "Unauthorized access"),

            KeyNotFoundException =>
                (StatusCodes.Status404NotFound, "Resource not found"),

            PostgresException pgEx when pgEx.Severity == "ERROR" =>
                (StatusCodes.Status400BadRequest, pgEx.MessageText),

            ArgumentNullException argEx =>
                (StatusCodes.Status400BadRequest, $"Missing argument: {argEx.ParamName}"),

            ArgumentException argEx =>
                (StatusCodes.Status400BadRequest, argEx.Message),

            InvalidOperationException opEx =>
                (StatusCodes.Status409Conflict, opEx.Message),

            DbUpdateException dbEx =>
                (StatusCodes.Status500InternalServerError,
                 dbEx.InnerException?.Message ?? dbEx.Message),

            _ =>
                (StatusCodes.Status500InternalServerError,
                 "An internal server error occurred")
        };
    }
}