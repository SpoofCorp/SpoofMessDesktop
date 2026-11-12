using System.ComponentModel;

namespace DeviceInfoLibrary.Results;

public record class Result(
        bool Success,
        string Message,
        string Error,
        int StatusCode
    ) : BaseResult(
        Success,
        Message,
        Error,
        StatusCode)
{
    private static Result GetResult(bool success = true, string message = "", string error = "", int statusCode = 200) =>
        new(
            success,
            message,
            error,
            statusCode
        );

    [Obsolete("Используйте специализированные методы.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Result SuccessResult(string message = "Ok", int statusCode = 200) =>
        GetResult(message: message, statusCode: statusCode);

    #pragma warning disable CS0618
    public static Result OkResult(string message = "Ok") =>
        SuccessResult(message);

    public static Result DeletedResult(string message = "Deleted") =>
        SuccessResult(message, 204);
    #pragma warning restore CS0618

    public static Result Forbidden(string error = "Forbidden") =>
        ErrorResult(error, 403);

    public static Result NotFoundResult(string error) =>
        ErrorResult(error, 404);

    public static Result BadRequest(string error) =>
        ErrorResult(error, 400);

    public static Result UnAuthorized(string error) =>
        ErrorResult(error, 401);

    public static Result InternalServerError(string error = "Internal server error") =>
        ErrorResult(error, 500);

    public static Result ErrorResult(string error, int statusCode = 500) =>
        GetResult(success: false, error: error, statusCode: statusCode);

    public static Result From<T>(Result<T> result) =>
        GetResult(result, result.Message, result.Error, result.StatusCode);

    public static Result Parse(string message, int statusCode)
    {
        return statusCode switch
        {
            200 => OkResult(message),
            204 => DeletedResult(message),
            400 => BadRequest(message),
            401 => UnAuthorized(message),
            403 => Forbidden(message),
            404 => NotFoundResult(message),
            500 => ErrorResult(message),
            _ => throw new InvalidOperationException(message),
        };
    }

    public static implicit operator bool(Result result) => result.Success;
    public override string ToString() =>
        Success ? Message : Error;
}
