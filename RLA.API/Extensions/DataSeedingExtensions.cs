using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RLA.Domain.Entities;
using RLA.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace RLA.API.Extensions
{
    public static class DataSeedingExtensions
    {
        public static async Task SeedDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("RLA.Web.Extensions.DataSeedingExtensions");

            try
            {
                logger.LogInformation("Creating roles before anything else...");
                await EnsureRolesCreated(services);
                logger.LogInformation("Roles created successfully.");

                logger.LogInformation("Starting data seeding...");
                await SeedData.InitializeAsync(services); // Changed to InitializeAsync
                logger.LogInformation("Data seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private static async Task EnsureRolesCreated(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("RLA.Web.Extensions.DataSeedingExtensions");
            string[] roles = { "Admin", "User", "Professor", "Student", "Assistant" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    logger.LogInformation("Creating role: {Role}", role);
                    var result = await roleManager.CreateAsync(new ApplicationRole { Name = role });
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Role {Role} created successfully.", role);
                    }
                    else
                    {
                        logger.LogError("Failed to create role {Role}: {Errors}", role, string.Join(", ", result.Errors.Select(e => e.Description)));
                        throw new InvalidOperationException($"Failed to create role {role}");
                    }
                }
                else
                {
                    logger.LogInformation("Role {Role} already exists.", role);
                }
            }
        }
    }
}