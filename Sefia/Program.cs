using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sefia.Common;
using Sefia.Services;
using Sefia.Data;

namespace Sefia
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddEnvironmentVariables();
            CheckEnvironmentVariables(builder.Configuration);

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
                options.AddPolicy("AllowedOrigins", corsBuilder =>
                {
                    var allowedHosts = GetAllowedHosts(builder.Configuration);
                    if (allowedHosts.Any())
                    {
                        corsBuilder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .WithOrigins(allowedHosts.ToArray());
                    }
                    else
                    {
                        throw new InvalidOperationException("No valid hosts were found for CORS configuration.");
                    }
                });
            });

            builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
            {
                string CONNECTION_STRING = builder.Configuration["CONNECTION_STRING"]!;
                optionsBuilder.UseMySql(CONNECTION_STRING, ServerVersion.AutoDetect(CONNECTION_STRING));
            });

            builder.Services.AddAuthorization(config =>
            {
                config.AddPolicy(UserRoles.Admin, Policies.AdminPolicy());
                config.AddPolicy(UserRoles.User, Policies.UserPolicy());
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["JWT_SECRET_KEY"]!)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddSingleton<AppSettingsService>();
            builder.Services.AddScoped<UserService>();

            var app = builder.Build();

            var appSettingsService = app.Services.GetRequiredService<AppSettingsService>();
            appSettingsService.LoadFromFile();

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

            var missingEnvVariables = requiredEnvVariables
                .Where(key => string.IsNullOrEmpty(configuration[key]))
                .ToList();

            if (missingEnvVariables.Any())
            {
                Console.WriteLine("Missing required environment variables: {Variables}", string.Join(", ", missingEnvVariables));
                throw new InvalidOperationException($"Missing environment variables: {string.Join(", ", missingEnvVariables)}");
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
