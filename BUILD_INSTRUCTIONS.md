# Build Instructions

## Prerequisites

1. **.NET 8 SDK** - Download from https://dotnet.microsoft.com/download/dotnet/8.0
2. **Visual Studio 2022** (Community, Professional, or Enterprise) OR **Visual Studio Code** with C# Dev Kit

## Building the Project

### Using Visual Studio

1. Open `Img2Go.sln` in Visual Studio 2022
2. Restore NuGet packages (usually automatic, or right-click solution → Restore NuGet Packages)
3. Build the solution (F6 or Build → Build Solution)
4. Run the application (F5)

### Using Command Line

```bash
# Navigate to project directory
cd Img2Go

# Restore dependencies
dotnet restore

# Build in Debug mode
dotnet build

# Build in Release mode (optimized)
dotnet build --configuration Release

# Run the application
dotnet run

# Publish as self-contained executable
dotnet publish -c Release -r win-x64 --self-contained true
```

## Project Structure

```
Img2Go/
├── Controls/              # Custom WPF controls
│   ├── ImagePreviewControl.xaml
│   └── ImagePreviewControl.xaml.cs
├── Converters/            # Value converters for data binding
│   ├── InverseBooleanConverter.cs
│   ├── NullToVisibilityConverter.cs
│   └── NullableIntConverter.cs
├── Models/                # Data models
│   └── ImageItem.cs
├── Services/              # Core business logic
│   ├── ConversionService.cs
│   ├── SettingsService.cs
│   ├── ServiceLocator.cs
│   └── ThemeService.cs
├── Styles/                # XAML resource dictionaries
│   ├── Colors.xaml
│   ├── DarkTheme.xaml
│   ├── LightTheme.xaml
│   └── Themes.xaml
├── Views/                 # WPF views/windows
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   ├── SplashScreen.xaml
│   └── SplashScreen.xaml.cs
├── ViewModels/            # MVVM view models
│   └── MainViewModel.cs
├── App.xaml
├── App.xaml.cs
├── Img2Go.csproj
└── Img2Go.sln
```

## Troubleshooting

### Missing Packages
If you encounter package restore errors:
```bash
dotnet restore --force
```

### Build Errors
- Ensure you have .NET 8 SDK installed: `dotnet --version` should show 8.x
- Check that all NuGet packages are restored
- Verify Visual Studio has the .NET desktop development workload installed

### Runtime Errors
- Ensure .NET 8 Runtime is installed on the target machine
- For self-contained builds, all dependencies are included in the output

## Publishing for Distribution

### Portable Version
```bash
dotnet publish -c Release -r win-x64 --self-contained false
```
Output will be in: `bin/Release/net8.0-windows/win-x64/publish/`

### Self-Contained (Includes .NET Runtime)
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```
Creates a single executable file.

## Notes

- The app stores settings in `%LocalAppData%\Img2Go\config.json`
- All image processing is done 100% offline
- Supported formats: JPG, PNG, WEBP, BMP, TIFF, HEIC/HEIF (converted to JPEG)

