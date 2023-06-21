using API.Handlers;

namespace API.Extentions
{
    public static class EnableRequestRewindMiddlewareExtention
    {
        public static void UseEnableRequestRewindMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<EnableRequestRewindMiddleware>();
        }
    }
}
