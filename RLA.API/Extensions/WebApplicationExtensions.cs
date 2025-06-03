using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace RLA.Web.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigureMiddleware(this WebApplication app)
        {
            var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("RLA.Web.Extensions.WebApplicationExtensions");

            if (app.Environment.IsDevelopment())
            {
                logger.LogInformation("Running in Development mode");
            }
            else
            {
                logger.LogInformation("Running in Production mode");
            }

            app.UseCors("AllowAll");
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}