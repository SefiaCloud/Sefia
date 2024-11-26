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
                    corsBuilder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowAnyOrigin();
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

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }

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
    }
}
