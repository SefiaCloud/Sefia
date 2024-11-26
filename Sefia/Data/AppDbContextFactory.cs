using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sefia.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        LoadEnvironmentVariables();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("The CONNECTION_STRING environment variable is not set.");
        }

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        return new AppDbContext(optionsBuilder.Options);
    }

    private void LoadEnvironmentVariables()
    {
        var launchSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "Properties", "launchSettings.json");
        if (File.Exists(launchSettingsPath))
        {
            var json = File.ReadAllText(launchSettingsPath);
            var launchSettings = System.Text.Json.JsonDocument.Parse(json);

            foreach (var profile in launchSettings.RootElement.GetProperty("profiles").EnumerateObject())
            {
                if (profile.Value.TryGetProperty("environmentVariables", out var envVars))
                {
                    foreach (var envVar in envVars.EnumerateObject())
                    {
                        Environment.SetEnvironmentVariable(envVar.Name, envVar.Value.GetString());
                    }
                }
            }
        }
    }
}
