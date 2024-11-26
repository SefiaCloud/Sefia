using Newtonsoft.Json;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Sefia.Common;

namespace Sefia.Services;

public class AppSettingsService
{
    private AppSettings _appSettings; // AppSettings instance managed internally
    private readonly string _filePath;
    private readonly ILogger<AppSettingsService> _logger;

    public AppSettingsService(ILogger<AppSettingsService> logger)
    {
        _logger = logger;
        _filePath = "app_settings.json"; // Path to the settings file
        _appSettings = new AppSettings(); // Initialize AppSettings internally
    }

    /// <summary>
    /// Loads settings from the file. This method is called during initial loading.
    /// </summary>
    public void LoadFromFile()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                var loadedSettings = JsonConvert.DeserializeObject<AppSettings>(json);

                if (loadedSettings != null)
                {
                    _appSettings = loadedSettings; // Replace current instance with loaded settings
                    _logger.LogInformation("Settings loaded from file.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading settings from file.");
                throw;
            }
        }
        else
        {
            _logger.LogWarning("Settings file not found. Using default values.");
        }
    }

    /// <summary>
    /// Saves the current settings to the file.
    /// </summary>
    public void SaveToFile()
    {
        try
        {
            var json = JsonConvert.SerializeObject(_appSettings, Formatting.Indented);
            File.WriteAllText(_filePath, json);
            _logger.LogInformation("Settings saved to file.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save settings to file.");
            throw;
        }
    }

    /// <summary>
    /// Initializes the settings.
    /// </summary>
    public void Initialize()
    {
        if (!_appSettings.IsInitialized)
        {
            _appSettings.IsInitialized = true;

            _logger.LogInformation("AppSettings has been initialized.");
        }
    }

    /// <summary>
    /// Retrieves the value of a specific setting by its name.
    /// </summary>
    /// <param name="key">The name of the setting.</param>
    /// <returns>The value of the setting, or null if not found.</returns>
    public object? Get(string key)
    {
        var property = typeof(AppSettings).GetProperty(key);
        if (property != null)
        {
            _logger.LogDebug($"Retrieving setting '{key}'.");
            return property.GetValue(_appSettings);
        }

        _logger.LogWarning($"Setting '{key}' not found.");
        return null;
    }

    /// <summary>
    /// Retrieves the value of a specific setting by its name and attempts to cast it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to which the value should be cast.</typeparam>
    /// <param name="key">The name of the setting.</param>
    /// <returns>The value of the setting cast to the specified type, or default(T) if the cast fails.</returns>
    /// <exception cref="InvalidCastException">Thrown when the value cannot be converted to the specified type.</exception>
    public T Get<T>(string key)
    {
        var value = Get(key);

        if (value == null)
        {
            _logger.LogWarning($"Setting '{key}' is null or not found.");
            return default!;
        }

        try
        {
            if (value is T castValue)
            {
                _logger.LogDebug($"Successfully retrieved and cast setting '{key}' to {typeof(T)}.");
                return castValue;
            }

            // Attempt conversion if it's not a direct cast
            var convertedValue = (T)Convert.ChangeType(value, typeof(T));
            _logger.LogDebug($"Successfully converted setting '{key}' to {typeof(T)}.");
            return convertedValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to cast or convert setting '{key}' to {typeof(T)}.");
            throw new InvalidCastException($"Cannot cast or convert setting '{key}' to type {typeof(T)}.", ex);
        }
    }


    /// <summary>
    /// Updates the value of a specific setting by its name.
    /// </summary>
    /// <param name="key">The name of the setting.</param>
    /// <param name="value">The new value for the setting.</param>
    /// <returns>True if the setting was successfully updated, otherwise false.</returns>
    public bool Set(string key, object value)
    {
        var property = typeof(AppSettings).GetProperty(key);
        if (property != null)
        {
            try
            {
                // Attempt to set the property value dynamically
                property.SetValue(_appSettings, Convert.ChangeType(value, property.PropertyType));
                _logger.LogInformation($"Setting '{key}' updated to '{value}'.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update setting '{key}'.");
                throw;
            }
        }

        _logger.LogWarning($"Setting '{key}' not found.");
        return false;
    }

    /// <summary>
    /// Prints the current settings to the logs.
    /// </summary>
    public void PrintSettings()
    {
        _logger.LogInformation($"IsInitialized: {_appSettings.IsInitialized}");
    }
}
