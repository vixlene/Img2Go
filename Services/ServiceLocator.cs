namespace Img2Go.Services
{
    public static class ServiceLocator
    {
        public static ConversionService ConversionService { get; private set; }
        public static SettingsService SettingsService { get; private set; }
        public static ThemeService ThemeService { get; private set; }

        public static void Initialize()
        {
            ConversionService = new ConversionService();
            SettingsService = SettingsService.Instance;
            ThemeService = ThemeService.Instance;
        }
    }
}

