using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Sefia.Common;

namespace Sefia
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddEnvironmentVariables();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Description = "Bearer Authentication with JWT Token"
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins",
                    corsBuilder =>
                    {
                        var allowedHosts = GetAllowedHosts(builder.Configuration);

                        corsBuilder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .WithOrigins(allowedHosts.ToArray());
                    });
            });

            CheckEnvironmentVariables(builder.Configuration);

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("AllowedOrigins");

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

        private static void CheckEnvironmentVariables(IConfiguration configuration)
        {
            var requiredEnvVariables = new List<string>
            {
                "HOSTS",
                "JWT_SECRET_KEY",
                "REDIS_CONNECTION_STRING",
                "CONNECTION_STRING",
            };

            var missingEnvVariables = new List<string>();

            foreach (var key in requiredEnvVariables)
            {
                var value = configuration[key];
                if (string.IsNullOrEmpty(value))
                {
                    missingEnvVariables.Add(key);
                }
            }

            if (missingEnvVariables.Count > 0)
            {
                throw new InvalidOperationException(
                    $"The following environment variables are missing: {string.Join(", ", missingEnvVariables)}. Please set them and try again.");
            }

            Console.WriteLine("All required environment variables are properly set.");
        }

        private static List<string> GetAllowedHosts(IConfiguration configuration)
        {
            var hosts = configuration["HOSTS"];
            if (string.IsNullOrEmpty(hosts))
            {
                throw new InvalidOperationException("HOSTS environment variable is missing or empty.");
            }

            var allowedHosts = hosts.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

            if (allowedHosts.Count == 0)
            {
                throw new InvalidOperationException("No valid hosts were found in the HOSTS environment variable.");
            }

            Console.WriteLine($"Allowed hosts: {string.Join(", ", allowedHosts)}");
            return allowedHosts;
        }
    }
}
