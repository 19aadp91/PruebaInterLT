using System.Text.Json.Serialization;

namespace Domain.Common;

public enum ErrorType
{
    None = 0,
    Validation = 1,
    BadRequest = 2,
    Unauthorized = 3,
    Forbidden = 4,
    NotFound = 5,
    Conflict = 6,
    InternalServerError = 7
}

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = [];

    [JsonPropertyName("result")]
    public T? Data { get; set; }

    public ErrorType ErrorType { get; set; } = ErrorType.None;

    public static Result<T> Success(T data, string message = "Operation completed successfully.")
    {
        return new Result<T>
        {
            IsSuccess = true,
            Message = message,
            ErrorType = ErrorType.None,
            Errors = [],
            Data = data
        };
    }

    public static Result<T> Failure(string message, ErrorType errorType = ErrorType.Validation, List<string>? errors = null)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Message = message,
            ErrorType = errorType,
            Errors = errors ?? [],
            Data = default
        };
    }

    public static Result<T> Failure(string message, List<string>? errors) =>
        Failure(message, ErrorType.Validation, errors);
}

public class Result : Result<object?>
{
    public static Result Success(string message = "Operation completed successfully.")
    {
        return new Result
        {
            IsSuccess = true,
            Message = message,
            ErrorType = ErrorType.None,
            Errors = [],
            Data = null
        };
    }

    public static new Result Failure(string message, ErrorType errorType = ErrorType.Validation, List<string>? errors = null)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            ErrorType = errorType,
            Errors = errors ?? [],
            Data = null
        };
    }

    public static new Result Failure(string message, List<string>? errors) =>
        Failure(message, ErrorType.Validation, errors);
}
