using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Aplicacion.Core;
public class Result<T>
{
    public bool IsSuccess { get; set; }

    public T? Value { get; set; }

    public string? Error { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public static Result<T> Success(T value) => new Result<T>
    {
        IsSuccess = true,
        Value = value,
        StatusCode = HttpStatusCode.OK
    };

    public static Result<T> Failure(string error, HttpStatusCode statusCode = HttpStatusCode.BadRequest) => new Result<T>
    {
        IsSuccess = false,
        Error = error,
        StatusCode = statusCode
    };
}
