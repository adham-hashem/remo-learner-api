using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RLA.Domain.Entities;
using RLA.Infrastructure.Data;
using System.Text;

namespace RLA.API.Extensions
    {
        public static class ServiceCollectionExtensions
        {
            public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
            {
                // Configure logging
                services.AddLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Information);
                });

                // Configure database
                var connectionString = Environment.GetEnvironmentVariable("RLA_DEV_DATABASE")
                ?? throw new InvalidOperationException("Database connection string 'RLA_DEV_DATABASE' is missing.");
                Console.WriteLine(new string('#', 20));
                Console.WriteLine($"Loaded connection string: {connectionString}");
                Console.WriteLine(new string('#', 20));
                services.AddDbContext<ElearningPlatformDbContext>(options =>
                    options.UseSqlServer(connectionString));

                // Configure Identity
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

                // Configure JWT Authentication
                var jwtSecretKey = Environment.GetEnvironmentVariable("RLA_DEV_JWT_SECRET")
                    ?? throw new InvalidOperationException("JWT Secret Key 'RLA_DEV_JWT_SECRET' is missing.");

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
                    options.MapInboundClaims = false;
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                            var logger = loggerFactory.CreateLogger("RLA.API.Extensions.ServiceCollectionExtensions");
                            logger.LogError(context.Exception, "Authentication failed. Token: {Token}", context.Request.Headers["Authorization"]!);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                            var logger = loggerFactory.CreateLogger("RLA.API.Extensions.ServiceCollectionExtensions");
                            logger.LogInformation("Token validated successfully for user: {User}", context.Principal?.Identity?.Name);
                            return Task.CompletedTask;
                        }
                    };
                });

                // Configure Authorization and Controllers
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

                services.AddEndpointsApiExplorer();
                ConfigureCors(services);

                return services;
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
        }
    }