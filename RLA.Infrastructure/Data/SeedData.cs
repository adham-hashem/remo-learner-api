using RLA.Infrastructure.Data;
using RLA.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RLA.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = new ElearningPlatformDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ElearningPlatformDbContext>>());
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("RLA.Infrastructure.Data.SeedData");

            // Ensure database configuration is created
            await context.Database.EnsureCreatedAsync();

            // Seed Users (Admin)
            if (!context.Users.Any())
            {
                logger.LogInformation("Seeding admin user...");
                var adminUsername = Environment.GetEnvironmentVariable("ADMIN_USERNAME");
                var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
                var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? adminUsername; // Fallback to username if email not set

                if (string.IsNullOrWhiteSpace(adminUsername))
                {
                    logger.LogError("ADMIN_USERNAME environment variable is missing or empty.");
                    throw new Exception("Admin user seeding failed: Missing ADMIN_USERNAME.");
                }

                if (string.IsNullOrWhiteSpace(adminPassword))
                {
                    logger.LogError("ADMIN_PASSWORD environment variable is missing or empty.");
                    throw new Exception("Admin user seeding failed: Missing ADMIN_PASSWORD.");
                }

                if (string.IsNullOrWhiteSpace(adminEmail) || !adminEmail.Contains("@"))
                {
                    logger.LogError("ADMIN_EMAIL environment variable is missing, empty, or invalid: {Email}", adminEmail);
                    throw new Exception("Admin user seeding failed: Invalid or missing ADMIN_EMAIL.");
                }

                var adminUser = new ApplicationUser
                {
                    FullName = "admin1",
                    UniversityId = "ADMIN-001",
                    UserName = adminUsername,
                    Email = adminEmail,
                    PhoneNumber = "987-654-3210",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                {
                    logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    throw new Exception("Admin user creation failed during seeding.");
                }

                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Created ApplicationUser (Admin) with ID: {UserId}", adminUser.Id);

                await context.SaveChangesAsync();
                logger.LogInformation("Seeded admin user successfully.");

                // Verify Admin user exists in the database
                var savedUser = await context.Users.FindAsync(adminUser.Id);
                if (savedUser == null)
                {
                    logger.LogError("Admin user with Id: {UserId} was not saved correctly.", adminUser.Id);
                    throw new Exception("Admin user seeding failed.");
                }
            }

            // Seed Levels
            if (!context.Levels.Any())
            {
                var level = new Level { Id = Guid.NewGuid() };
                context.Levels.Add(level);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded level with ID: {LevelId}", level.Id);
            }

            // Seed Terms (optional, kept for potential future use)
            if (!context.Terms.Any())
            {
                var term = new Term
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(4)
                };
                context.Terms.Add(term);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded term with ID: {TermId}", term.Id);
            }
        }
    }
}