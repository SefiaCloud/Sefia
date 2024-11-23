namespace Sefia.Common;

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Success(string message = "Request successful")
        => new ApiResponse { Result = true, Message = message };

    public static ApiResponse Error(string message, params string[] errors)
        => new ApiResponse
        {
            Result = false,
            Message = message,
            Errors = errors
        };
}

public class ApiResponse<T>
{
    public required bool Result { get; init; }
    public required string Message { get; init; }
    public T? Data { get; init; } = default;
    public IEnumerable<string> Errors { get; init; } = Enumerable.Empty<string>();

    protected ApiResponse() { }

    public static ApiResponse<T> Success(T data, string message = "Request successful")
        => new ApiResponse<T>
        {
            Result = true,
            Data = data,
            Message = message
        };

    public static ApiResponse<T> Error(string message, string error)
        => new ApiResponse<T>
        {
            Result = false,
            Message = message,
            Errors = [error]
        };

    public static ApiResponse<T> Error(string message, IEnumerable<string> errors)
        => new ApiResponse<T>
        {
            Result = false,
            Message = message,
            Errors = errors
        };

    public static ApiResponse<T> Error(string message, Exception exception)
        => new ApiResponse<T>
        {
            Result = false,
            Message = message,
            Errors = [exception.Message]
        };

    public static ApiResponse<T> Error(string message, IEnumerable<Exception> exceptions)
        => new ApiResponse<T>
        {
            Result = false,
            Message = message,
            Errors = exceptions.Select(e => e.Message)
        };
}
