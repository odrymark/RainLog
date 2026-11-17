namespace Api.Common;

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }

    public static ServiceResponse<T> Success(T data)
    {
        return new ServiceResponse<T>
        {
            Data = data,
            IsSuccess = true,
            ErrorCode = null,
            ErrorMessage = null
        };
    }

    public static ServiceResponse<T> Failure(string errorCode, string errorMessage)
    {
        return new ServiceResponse<T>
        {
            Data = default,
            IsSuccess = false,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage
        };
    }
}