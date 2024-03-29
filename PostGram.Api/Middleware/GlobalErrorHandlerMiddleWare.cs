﻿using PostGram.Common.Exceptions;
using LogLevel = NLog.LogLevel;

namespace PostGram.Api.Middleware
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
                await ResponseHandler(context, e, LogLevel.Warn, 400);
            }
            catch (UnprocessableRequestPostGramException e)
            {
                await ResponseHandler(context, e, LogLevel.Warn, 422);
            }
            catch (NotFoundPostGramException e)
            {
                await ResponseHandler(context, e, LogLevel.Warn, 404);
            }
            catch (AuthorizationPostGramException e)
            {
                await ResponseHandler(context, e, LogLevel.Warn, 401);
            }
            catch (PostGramException e)
            {
                await ResponseHandler(context, e, LogLevel.Error, 500);
            }
            catch (Exception e)
            {
                await ResponseHandler(context, e, LogLevel.Fatal, 500);
            }
        }

        private async Task ResponseHandler(HttpContext context, Exception e, LogLevel logLevel, int httpStatusCode)
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