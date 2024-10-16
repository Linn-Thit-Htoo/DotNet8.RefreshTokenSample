﻿namespace DotNet8.RefreshTokenSample.Api.Utils;

public class Result<T>
{
    public T Data { get; set; }
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
    public EnumStatusCode StatusCode { get; set; }

    public static Result<T> Success(string message = "Success.") =>
        new Result<T>
        {
            Message = message,
            IsSuccess = true,
            StatusCode = EnumStatusCode.Success
        };

    public static Result<T> Success(T data, string message = "Success.") =>
        new Result<T>
        {
            Message = message,
            Data = data,
            IsSuccess = true,
            StatusCode = EnumStatusCode.Success
        };

    public static Result<T> Fail(
        string message = "Fail.",
        EnumStatusCode statusCode = EnumStatusCode.BadRequest
    ) =>
        new Result<T>
        {
            Message = message,
            IsSuccess = false,
            StatusCode = statusCode
        };

    public static Result<T> Fail(Exception ex) =>
        Result<T>.Fail(ex.ToString(), EnumStatusCode.InternalServerError);

    public static Result<T> Unauthorized(string message = "Unauthorized.") =>
        Result<T>.Fail(message, EnumStatusCode.UnAuthorized);

    public static Result<T> NotFound(string message = "No data found.") =>
        Result<T>.Fail(message, EnumStatusCode.NotFound);

    public static Result<T> Duplicate(string message = "Duplicate data.") =>
        Result<T>.Fail(message, EnumStatusCode.Conflict);
}
