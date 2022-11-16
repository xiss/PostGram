using PostGram.Common.Exceptions;
using LogLevel = NLog.LogLevel;

namespace PostGram.Api.Middlewares
{
    public class GlobalErrorHandlerMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly NLog.Logger _logger;
        public GlobalErrorHandlerMiddleWare(RequestDelegate next)
        {
            _next = next;
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BadRequestPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(e.Message);
            }
            catch (NotFoundPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(e.Message);
            }
            catch (DbPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(e.Message);
            }
            catch (AuthorizationPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(e.Message);
            }
            catch (FilePostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                context.Response.StatusCode = 500;
                if (e.InnerException != null)
                    await context.Response.WriteAsJsonAsync(e.InnerException.Message);
                await context.Response.WriteAsJsonAsync(e.Message);
            }
            catch (CriticalPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(e.Message);
            }
            catch (CommonPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(e.Message);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Fatal, e);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(e.Message);
            }
        }
    }

    public static class GlobalErrorHandlerMiddleWareExtension
    {
        public static IApplicationBuilder UseGlobalErrorHandlerMiddleWare(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalErrorHandlerMiddleWare>();
        }
    }

}