using PostGram.Common.Exceptions;

namespace PostGram.Api.Middlewares;

public class GlobalErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public GlobalErrorHandlerMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadRequestPostGramException e)
        {
            await ResponseHandler(context, e, LogLevel.Warning, 400);
        }
        catch (UnprocessableRequestPostGramException e)
        {
            await ResponseHandler(context, e, LogLevel.Warning, 422);
        }
        catch (NotFoundPostGramException e)
        {
            await ResponseHandler(context, e, LogLevel.Warning, 404);
        }
        catch (AuthorizationPostGramException e)
        {
            await ResponseHandler(context, e, LogLevel.Warning, 401);
        }
        catch (PostGramException e)
        {
            await ResponseHandler(context, e, LogLevel.Error, 500);
        }
        catch (Exception e)
        {
            await ResponseHandler(context, e, LogLevel.Critical, 500);
        }
    }

    private async Task ResponseHandler(HttpContext context, Exception e, LogLevel logLevel, int httpStatusCode)
    {
        _logger.Log(logLevel, e, null);
        context.Response.StatusCode = httpStatusCode;
        if (e.InnerException != null)
            await context.Response.WriteAsJsonAsync(e.InnerException.Message);
        else
            await context.Response.WriteAsJsonAsync(e.Message);
    }
}

public static class GlobalErrorHandlerMiddleWareExtension
{
    public static IApplicationBuilder UseGlobalErrorHandlerMiddleWare(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalErrorHandlerMiddleware>();
    }
}