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
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ElearningPlatformDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ElearningPlatformDbContext>>());
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("RLA.Infrastructure.Data.SeedData");

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Users (Admin)
            if (!context.Users.Any())
            {
                logger.LogInformation("Seeding admin user...");
                var adminUser = new ApplicationUser
                {
                    FullName = "admin1",
                    UniversityId = "ADMIN-001",
                    UserName = Environment.GetEnvironmentVariable("ADMIN_USERNAME"),
                    PhoneNumber = "987-654-3210",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, Environment.GetEnvironmentVariable("ADMIN_PASSWORD")!);
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

            // Seed Levels (optional, kept for potential future use)
            if (!context.Levels.Any())
            {
                var level = new Level { Id = Guid.NewGuid() }; // Minimal Level entity
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

            //// Seed Courses (commented out since it requires a Professor)
            //if (!context.Courses.Any())
            //{
            //    var professor = await context.Professors.FirstOrDefaultAsync();
            //    if (professor == null)
            //    {
            //        logger.LogError("No professor found in the database for course seeding.");
            //        throw new Exception("No professor exists for course seeding.");
            //    }

            //    var level = await context.Levels.FirstOrDefaultAsync();
            //    var term = await context.Terms.FirstOrDefaultAsync();

            //    if (level == null || term == null)
            //    {
            //        logger.LogError("Required entities (Level or Term) not found for course seeding.");
            //        throw new Exception("Missing required entities for course seeding.");
            //    }

            //    logger.LogInformation("Seeding course with ProfessorId: {ProfessorId}", professor.Id);
            //    var course = new Course
            //    {
            //        Name = "Introduction to Programming",
            //        Code = "CS101",
            //        Overview = "Basic programming concepts",
            //        DayOfWeek = DayOfWeek.Monday,
            //        Time = TimeOnly.FromDateTime(DateTime.Now),
            //        Location = "Room 101",
            //        CreditHours = 3,
            //        LevelId = level.Id,
            //        ProfessorId = professor.Id,
            //        TermId = term.Id
            //    };

            //    context.Courses.Add(course);
            //    await context.SaveChangesAsync();
            //    logger.LogInformation("Seeded course with ID: {CourseId}, ProfessorId: {ProfessorId}", course.Id, course.ProfessorId);
            //}
        }
    }
}