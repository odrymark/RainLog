namespace Api.Common;

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}