using System.Windows;

namespace Img2Go
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Initialize services
            Services.ServiceLocator.Initialize();
            
            // Load settings
            Services.SettingsService.Instance.LoadSettings();
            
            // Apply theme
            Services.ThemeService.Instance.ApplyTheme();
        }
    }
}

