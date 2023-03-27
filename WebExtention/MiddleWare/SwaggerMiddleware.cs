using Microsoft.AspNetCore.Builder;
using WebUtils;

namespace WebExtention.Middlewares
{
    /// <summary>
    /// Swagger中间件
    /// </summary>
    public static class SwaggerMiddleware
    {
        public static void UseSwaggerMiddle(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}
