using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace WebApplication.todo.exceptions;

public sealed class GlobalExceptionHandling(ILogger<GlobalExceptionHandling> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandling> _logger = logger;

    //public ValueTask<bool> TryHandleAsync(
    //    HttpContext httpContext,
    //    Exception exception,
    //    CancellationToken cancellationToken)
    //{
    //    var handled = exception switch
    //    {
    //        ResourceNotFoundException notFound => HandleNotFound(httpContext, notFound, cancellationToken),
    //        ValidationException validation => HandleValidation(httpContext, validation, cancellationToken),
    //        _ => HandleGeneric(httpContext, exception, cancellationToken)
    //    };

    //    return ValueTask.FromResult(handled);
    //}

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken) 
    {
        ErrorResponse response;
        int statusCode;

        switch(exception) {
            case ResourceNotFoundException notFound:
                statusCode = StatusCodes.Status404NotFound;
                response = new ErrorResponse("NOT_FOUND", notFound.Message);
                break;

            case BadHttpRequestException badRequest:
                statusCode = StatusCodes.Status400BadRequest;
                response = new ErrorResponse("VALIDATION_ERROR", badRequest.Message);
                break;

            default:
                _logger.LogError(exception, "Unexpected error");

                statusCode = StatusCodes.Status500InternalServerError;
                response = new ErrorResponse("INTERNAL_SERVER_ERROR", "An unexpected error occurred");
                break;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }

    // -------------------------------------------------------------------------
    // TODO G — Handle ResourceNotFoundException
    // -------------------------------------------------------------------------
    // When a todo is not found, return 404 NOT_FOUND with an ErrorResponse.
    //
    // Hint:
    //   return Results.NotFound(new ErrorResponse("NOT_FOUND", exception.Message));
    // -------------------------------------------------------------------------
    private static bool HandleNotFound(
        HttpContext httpContext,
        ResourceNotFoundException exception,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("TODO G: implement me");
    }

    // -------------------------------------------------------------------------
    // TODO H — Handle ValidationException
    // -------------------------------------------------------------------------
    // When validation fails on a request body, return 400 BAD_REQUEST with a
    // VALIDATION_ERROR response that lists all field errors in the message.
    //
    // Hint: validation failures contain the field names and messages. Join them:
    //       "title: Title is required, dueDate: must not be null"
    // -------------------------------------------------------------------------
    private static bool HandleValidation(
        HttpContext httpContext,
        ValidationException exception,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("TODO H: implement me");
    }

    // -------------------------------------------------------------------------
    // TODO I — Handle all other exceptions (catch-all)
    // -------------------------------------------------------------------------
    // For any unexpected error, return 500 INTERNAL_SERVER_ERROR.
    // IMPORTANT: log the full exception server-side, but return a safe generic
    // message to the client — never expose internal stack traces!
    //
    // Hint: use the injected logger:
    //   logger.LogError(exception, "Unexpected error");
    // -------------------------------------------------------------------------
    private bool HandleGeneric(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("TODO I: implement me");
    }
}
