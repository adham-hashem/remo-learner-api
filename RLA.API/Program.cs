using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RLA.API.Extensions;
using RLA.Web.Extensions;
using System;
using System.Threading.Tasks;

namespace RLA.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Load environment variables
            Env.Load();

            // Create and configure the application
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.ConfigureServices(builder.Configuration);

            var app = builder.Build();
            app.ConfigureMiddleware();

            // Seed roles and data
            await app.SeedDataAsync();

            await app.RunAsync();
        }
    }
}