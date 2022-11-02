using System.Net;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            //TODO почему сюда не выбрасывает все ошибки?
            await HandleExceptionMessageAsync(context, ex).ConfigureAwait(false);
        }
    }
    private static Task HandleExceptionMessageAsync(HttpContext context, Exception exception)
    {
        return new Task(null);
        //context.Response.ContentType = "application/json";
        //int statusCode = (int)HttpStatusCode.InternalServerError;
        //var result = JsonConvert.SerializeObject(new
        //{
        //    StatusCode = statusCode,
        //    ErrorMessage = exception.Message
        //});
        //context.Response.ContentType = "application/json";
        //context.Response.StatusCode = statusCode;
        //return context.Response.WriteAsync(result);
    }
}
