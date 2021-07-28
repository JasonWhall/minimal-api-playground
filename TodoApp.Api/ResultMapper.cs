using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public static class ResultMapper
{
    public static IResult BadRequest() => new StatusCodeResult(StatusCodes.Status400BadRequest);

    public static IResult NotFound() => new StatusCodeResult(StatusCodes.Status404NotFound);

    public static IResult NoContent() => new StatusCodeResult(StatusCodes.Status204NoContent);

    public static IResult Created() => new StatusCodeResult(StatusCodes.Status201Created);

    public static OkResult<T> Ok<T>(T value) => new(value);

    public class OkResult<T> : IResult
    {
        private readonly T _value;

        public OkResult(T value)
        {
            _value = value;
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
            return httpContext.Response.WriteAsJsonAsync(_value);
        }
    }
}