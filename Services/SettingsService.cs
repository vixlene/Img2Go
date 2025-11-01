using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Img2Go.Services
{
    public class SettingsService
    {
        private static readonly Lazy<SettingsService> _instance = new Lazy<SettingsService>(() => new SettingsService());
        public static SettingsService Instance => _instance.Value;

        private readonly string _configPath;
        private AppSettings _settings;

        private SettingsService()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Img2Go");

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            _configPath = Path.Combine(appDataPath, "config.json");
            _settings = new AppSettings();
        }

        public AppSettings Settings => _settings;

        public void LoadSettings()
        {
            if (File.Exists(_configPath))
            {
                try
                {
                    var json = File.ReadAllText(_configPath);
                    _settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
                catch
                {
                    _settings = new AppSettings();
                }
            }
        }

        public void SaveSettings()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_settings, options);
                File.WriteAllText(_configPath, json);
            }
            catch
            {
                // Silent fail on save
            }
        }
    }

    public class AppSettings
    {
        public string Theme { get; set; } = "System";
        public string AccentColor { get; set; } = "#0078D4";
        public int DefaultQuality { get; set; } = 85;
        public bool MaintainAspectRatio { get; set; } = true;
        public string DefaultOutputFormat { get; set; } = "Jpeg";
        public string LastOutputDirectory { get; set; } = string.Empty;
    }
}

