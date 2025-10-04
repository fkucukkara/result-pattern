using API.Common;
using API.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Extensions;

public static class ResultExtensions
{
    public static Results<Ok, BadRequest<ErrorResponse>> ToHttpResponse(this Result<object?> result)
    {
        return result.IsSuccess
            ? TypedResults.Ok()
            : TypedResults.BadRequest(new ErrorResponse(result.Error));
    }

    public static Results<Ok<T>, BadRequest<ErrorResponse>> ToHttpResponse<T>(this Result<T> result)
    {
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.BadRequest(new ErrorResponse(result.Error));
    }

    public static Results<Ok<T>, NotFound<ErrorResponse>, BadRequest<ErrorResponse>> ToHttpResponseWithNotFound<T>(
        this Result<T> result)
    {
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        var isNotFound = result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase);

        return isNotFound
            ? TypedResults.NotFound(new ErrorResponse(result.Error))
            : TypedResults.BadRequest(new ErrorResponse(result.Error));
    }

    public static Results<Ok, NotFound<ErrorResponse>, BadRequest<ErrorResponse>> ToHttpResponseWithNotFound(
        this Result<object?> result)
    {
        if (result.IsSuccess)
            return TypedResults.Ok();

        var isNotFound = result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase);

        return isNotFound
            ? TypedResults.NotFound(new ErrorResponse(result.Error))
            : TypedResults.BadRequest(new ErrorResponse(result.Error));
    }
}
