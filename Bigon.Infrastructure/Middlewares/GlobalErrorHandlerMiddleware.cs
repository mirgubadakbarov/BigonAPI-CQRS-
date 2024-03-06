using Bigon.Infrastructure.Commons.Concrates;
using Bigon.Infrastructure.Exceptions;
using Bigon.Infrastructure.Localize.General;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace Bigon.Infrastructure.Middlewares
{
    public class GlobalErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                ApiResponse response = null;
                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy
                        {
                            ProcessDictionaryKeys = true,
                        }
                    },
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                };
                context.Response.ContentType = "application/json";


                switch (ex)
                {
                    case BadRequestException bre:
                        response = ApiResponse.Fail(bre.Errors, GeneralResource.ResourceManager.GetString(ex.Message), HttpStatusCode.BadRequest);
                        break;

                    case NotFoundException:
                        response = ApiResponse.Fail(GeneralResource.ResourceManager.GetString(ex.Message), HttpStatusCode.NotFound);
                        break;

                    default:
                        response = ApiResponse.Fail(ex.Message, HttpStatusCode.InternalServerError);

                        break;
                }
                context.Response.StatusCode = (int)response.StatusCode;
                var json = JsonConvert.SerializeObject(response, jsonSettings);
                await context.Response.WriteAsync(json);

            }

        }
    }

    public static class GlobalErrorHandlerMiddlewareExtension
    {
        public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalErrorHandlerMiddleware>();
            return app;
        }
    }
}
