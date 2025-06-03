using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace RLA.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigureMiddleware(this WebApplication app)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();

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