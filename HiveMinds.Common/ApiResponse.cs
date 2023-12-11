using System.Net;

namespace HiveMinds.Common;

public class ApiResponse<T> where T : class?
{
    public bool Success { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }

    public ApiResponse(bool success, HttpStatusCode statusCode, string message, T? data = null)
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
        Data = data;
    }

    public static ApiResponse<T> SuccessResponse(string message = "", T? data = null) =>
        new(true, HttpStatusCode.OK, message, data);

    public static ApiResponse<T> FailureResponse(HttpStatusCode statusCode, string message = "", T? data = null) =>
        new(false, statusCode, message, data);

    public static ApiResponse<T> Response(bool success, HttpStatusCode statusCode, string message, T? data = null) =>
        new(success, statusCode, message, data);
}