namespace FreelanceFlow.Core.Common;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public string? Error { get; protected set; }
    public List<string> Errors { get; protected set; } = new();

    protected Result(bool isSuccess, string? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        if (!string.IsNullOrEmpty(error))
        {
            Errors.Add(error);
        }
    }

    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(List<string> errors)
    {
        var result = new Result(false);
        result.Errors = errors;
        return result;
    }
}

public class Result<T> : Result
{
    public T? Value { get; protected set; }

    protected Result(T? value, bool isSuccess, string? error = null) : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(value, true);
    public static new Result<T> Failure(string error) => new(default, false, error);
    public static new Result<T> Failure(List<string> errors)
    {
        var result = new Result<T>(default, false);
        result.Errors = errors;
        return result;
    }
}