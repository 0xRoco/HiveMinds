namespace HiveMinds.Common;

public class Result<T, TStatus> where TStatus : Enum
{
    public T Value { get; }
    public TStatus Status { get; }
    public string Message { get; }
    public bool IsSuccess { get; }

    private Result(T value, TStatus status, string message, bool isSuccess)
    {
        Value = value;
        Status = status;
        Message = message;
        IsSuccess = isSuccess;
    }

    public static Result<T, TStatus> Success(T value, string message = "")
    {
        return new Result<T, TStatus>(value, default!, message, true);
    }

    public static Result<T, TStatus> Failure(string message, TStatus status)
    {
        return new Result<T, TStatus>(default!, status, message, false);
    }
}