using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Domain.Exceptions;

public class BadRequestException : HttpResponseException
{
    public BadRequestException(string message)
    {
        Value = message;
        Status = (int)HttpStatusCode.BadRequest;
    }

    public static void ThrowIfNull([NotNull] object? argument, string message = "Bad Request")
    {
        if (argument is null)
        {
            throw new BadRequestException(message);
        }
    }

    public static void ThrowIfEmpty(string? argument, string message = "Bad Request")
    {
        if (string.IsNullOrEmpty(argument))
        {
            throw new BadRequestException(message);
        }
    }

    public static void ThrowIfZero(int? argument, string message = "Bad Request")
    {
        if (argument.GetValueOrDefault(0) == 0)
        {
            throw new BadRequestException(message);
        }
    }

    public static void ThrowIfTrue(bool? argument, string message = "Bad Request")
    {
        if (argument == true)
        {
            throw new BadRequestException(message);
        }
    }

    public static void ThrowIfFalse(bool? argument, string message = "Bad Request") =>
        ThrowIfTrue(!argument, message);
}
