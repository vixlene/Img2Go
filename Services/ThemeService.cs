using System;
using System.Windows;
using System.Windows.Media;

namespace Img2Go.Services
{
    public class ThemeService
    {
        private static readonly Lazy<ThemeService> _instance = new Lazy<ThemeService>(() => new ThemeService());
        public static ThemeService Instance => _instance.Value;

        private ThemeService() { }

        public void ApplyTheme()
        {
            var settings = SettingsService.Instance.Settings;
            var theme = settings.Theme;

            if (theme == "System")
            {
                theme = IsSystemDarkMode() ? "Dark" : "Light";
            }

            Application.Current.Resources.MergedDictionaries.Clear();
            
            var themeDict = new ResourceDictionary();
            var source = theme == "Dark"
                ? new Uri("pack://application:,,,/Styles/DarkTheme.xaml")
                : new Uri("pack://application:,,,/Styles/LightTheme.xaml");
            
            themeDict.Source = source;
            Application.Current.Resources.MergedDictionaries.Add(themeDict);

            // Apply accent color
            if (!string.IsNullOrEmpty(settings.AccentColor))
            {
                try
                {
                    var accentColor = (Color)ColorConverter.ConvertFromString(settings.AccentColor);
                    Application.Current.Resources["AccentColor"] = accentColor;
                    Application.Current.Resources["AccentBrush"] = new SolidColorBrush(accentColor);
                }
                catch { }
            }
        }

        public void ToggleTheme()
        {
            var settings = SettingsService.Instance.Settings;
            settings.Theme = settings.Theme == "Dark" ? "Light" : "Dark";
            SettingsService.Instance.SaveSettings();
            ApplyTheme();
        }

        private bool IsSystemDarkMode()
        {
            try
            {
                var registry = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                var value = registry?.GetValue("AppsUseLightTheme");
                return value is int intValue && intValue == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}

