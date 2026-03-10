namespace Employee.Shared.Exceptions;

public class AppException(string message, int statusCode = 400) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

public class NotFoundException(string msg) : AppException(msg, 404)
{
}

public class UnauthorizedException(string msg) : AppException(msg, 401)
{
}