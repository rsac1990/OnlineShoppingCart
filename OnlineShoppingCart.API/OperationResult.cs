namespace OnlineShopping.Api;

public class OperationResult
{
    public bool IsSuccess { get; }
    public string ErrorMessage { get; }
    public ErrorType ErrorType { get; }

    protected OperationResult(bool isSuccess, string errorMessage, ErrorType errorType)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    // Success Factory
    public static OperationResult Success() => new(true, string.Empty, ErrorType.None);

    // Failure Factories
    public static OperationResult Failure(string errorMessage, ErrorType errorType = ErrorType.Validation)
        => new(false, errorMessage, errorType);

    public static OperationResult NotFound(string errorMessage = "Recurso no encontrado")
        => new(false, errorMessage, ErrorType.NotFound);
}

// Generic version when your method needs to return data (e.g., Created entity, DTO)
public class Result<T> : OperationResult
{
    public T? Value { get; }

    private Result(T? value, bool isSuccess, string errorMessage, ErrorType errorType)
        : base(isSuccess, errorMessage, errorType)
    {
        Value = value;
    }

    // Success Factory with Data
    public static Result<T> Success(T value) => new(value, true, string.Empty, ErrorType.None);

    // Failure Factories for Generic Result
    public new static Result<T> Failure(string errorMessage, ErrorType errorType = ErrorType.Validation)
        => new(default, false, errorMessage, errorType);

    public new static Result<T> NotFound(string errorMessage = "Recurso no encontrado")
        => new(default, false, errorMessage, ErrorType.NotFound);
}

// Enum to categorize errors for clean HTTP mapping
public enum ErrorType
{
    None,
    NotFound,
    Validation
}
