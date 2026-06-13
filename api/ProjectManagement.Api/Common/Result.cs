namespace ProjectManagement.Api.Common;

public class ResultError
{
    public string Code { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;

    public static ResultError From(string code, string message) => new() { Code = code, Message = message };
}

public class Result
{
    public bool Success { get; init; }
    public ResultError? Error { get; init; }

    public static Result Ok() => new() { Success = true };
    public static Result Fail(string code, string message) => new() { Success = false, Error = ResultError.From(code, message) };
    public static Result NotFound(string message = "Not found") => Fail("NOT_FOUND", message);
    public static Result Unauthorized(string message = "Unauthorized") => Fail("UNAUTHORIZED", message);
    public static Result Forbidden(string message = "Forbidden") => Fail("FORBIDDEN", message);
    public static Result BadRequest(string message) => Fail("BAD_REQUEST", message);
}

public class Result<T> : Result
{
    public T? Value { get; init; }

    public static Result<T> Ok(T value) => new() { Success = true, Value = value };
    public new static Result<T> Fail(string code, string message) => new() { Success = false, Error = ResultError.From(code, message) };
    public new static Result<T> NotFound(string message = "Not found") => Fail("NOT_FOUND", message);
    public new static Result<T> Unauthorized(string message = "Unauthorized") => Fail("UNAUTHORIZED", message);
    public new static Result<T> Forbidden(string message = "Forbidden") => Fail("FORBIDDEN", message);
    public new static Result<T> BadRequest(string message) => Fail("BAD_REQUEST", message);
}
