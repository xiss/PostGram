using System.Net;
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
                await ErorreHandlerFor500Code(context, e, LogLevel.Warn, 400);
            }
            catch (UnprocessableRequestPostGramException e)
            {
                await ErorreHandlerFor500Code(context, e, LogLevel.Warn, 422);
            }
            catch (NotFoundPostGramException e)
            {
                await ErorreHandlerFor500Code(context, e, LogLevel.Warn, 404);
            }
            catch (DbPostGramException e)
            {
                await ErorreHandlerFor500Code(context, e, LogLevel.Error, 500);
            }
            catch (AuthorizationPostGramException e)
            {
                await ErorreHandlerFor500Code(context, e, LogLevel.Warn, 401);
            }
            catch (FilePostGramException e)
            {
                await ErorreHandlerFor500Code(context, e, LogLevel.Error, 500);
            }
            catch (CriticalPostGramException e)
            {
                await ErorreHandlerFor500Code(context, e, LogLevel.Error, 500);
            }
            catch (CommonPostGramException e)
            {
                await ErorreHandlerFor500Code(context, e, LogLevel.Error, 500);
            }
            catch (Exception e)
            {
                await ErorreHandlerFor500Code(context, e, LogLevel.Fatal, 500);
            }
        }

        private async Task ErorreHandlerFor500Code(HttpContext context, Exception e, LogLevel logLevel, int httpStatusCode)
        {
            _logger.Log(logLevel, e);
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
            return builder.UseMiddleware<GlobalErrorHandlerMiddleWare>();
        }
    }

}