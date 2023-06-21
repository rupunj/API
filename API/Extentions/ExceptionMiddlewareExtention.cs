using API.Handlers;

namespace API.Extentions
{
    public static class ExceptionMiddlewareExtention
    {
        public static void UseExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
