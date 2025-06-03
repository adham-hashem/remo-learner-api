using DotNetEnv;
using RLA.Infrastructure.Data;
using RLA.Domain.Entities;
//using RLA.Infrastructure.Repositories.Contracts;
//using RLA.Infrastructure.Repositories.Implemenations;
//using ELP.Services.Contracts;
//using ELP.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Load environment variables from .env file
        Env.Load();

        // Create a temporary service provider for role seeding
        var tempBuilder = WebApplication.CreateBuilder(args);
        ConfigureServices(tempBuilder.Services, tempBuilder.Configuration);
        using (var tempApp = tempBuilder.Build())
        {
            using (var scope = tempApp.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    logger.LogInformation("Creating roles before anything else...");
                    await EnsureRolesCreated(services); // Roles created first
                    logger.LogInformation("Roles created successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while creating roles.");
                    throw; // Stop the app if role creation fails
                }
            }
        }

        // Proceed with normal application setup
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();
        ConfigureMiddleware(app);

        // Seed remaining data after roles are ensured
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            try
            {
                logger.LogInformation("Starting data seeding...");
                await SeedData.Initialize(services); // Seed data after roles
                logger.LogInformation("Data seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw; // Stop the app if seeding fails
            }
        }

        await app.RunAsync();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });

        var connectionString = Environment.GetEnvironmentVariable("RLA_DEV_DATABASE")
            ?? throw new InvalidOperationException("Database connection string 'RLA_DEV_DATABASE' is missing.");

        Console.WriteLine(new string('#', 20));
        Console.WriteLine($"Loaded connection string: {connectionString}");
        Console.WriteLine(new string('#', 20));

        services.AddDbContext<ElearningPlatformDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ElearningPlatformDbContext>()
        .AddDefaultTokenProviders()
        .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ElearningPlatformDbContext, Guid>>()
        .AddRoleStore<RoleStore<ApplicationRole, ElearningPlatformDbContext, Guid>>();

        services.AddScoped<UserManager<ApplicationUser>>();
        services.AddScoped<SignInManager<ApplicationUser>>();

        //services.AddScoped<IJwtServices, JwtServices>();
        //services.AddScoped<ITokenRepository, TokenRepository>();
        //services.AddScoped<IEmailService, EmailService>();

        var jwtSecretKey = Environment.GetEnvironmentVariable("RLA_DEV_JWT_SECRET")
            ?? throw new InvalidOperationException("JWT Secret Key 'ELP_DEV_JWT_SECRET' is missing.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Environment.GetEnvironmentVariable("RLA_DEV_JWT_ISSUER"),
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
            };
            options.MapInboundClaims = false;  // this is used to prevent mapping the user calims generated in JWT
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(context.Exception, "Authentication failed. Token: {Token}", context.Request.Headers["Authorization"]!);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Token validated successfully for user: {User}", context.Principal?.Identity?.Name);
                    return Task.CompletedTask;
                }

                // real example: Token validated successfully for user: Mohamed_gaber@elearningplatform.com
            };
        });

        services.AddAuthorization();
        services.AddControllers(options =>
        {
            options.Filters.Add(new ProducesAttribute("application/json"));
            options.Filters.Add(new ConsumesAttribute("application/json"));
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        })
        .AddXmlSerializerFormatters();


        //    services.AddControllers()
        //.AddJsonOptions(options =>
        //{
        //    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        //});

        services.AddEndpointsApiExplorer();
        ConfigureCors(services);
    }

    private static void ConfigureCors(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
        });
    }

    private static void ConfigureMiddleware(WebApplication app)
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
    }

    private static async Task EnsureRolesCreated(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var logger = services.GetRequiredService<ILogger<Program>>();
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














// ----------------------------------------------------

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
