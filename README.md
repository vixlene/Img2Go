# Img2Go
<<<<<<< HEAD
Lightweight, local Windows image converter built with .NET WPF
=======

A free, lightweight Windows app to batch convert and compress images 100% locally. Fast, clean, and privacy-first.

## ✨ Features

- ⚡ **Fast native .NET performance** - Optimized async/await processing
- 🧩 **Batch resize, compress, and convert** - Handle 1000+ images efficiently
- 🖼️ **Private & offline** - No uploads ever, everything runs on your machine
- 🌗 **Auto light/dark mode** - Matches your Windows theme preference
- 📁 **Drag-and-drop support** - Files and folders
- 🎨 **Multiple formats** - JPG, PNG, WEBP, HEIC, BMP, TIFF
- 🔧 **Customizable quality** - Adjustable compression slider
- 📏 **Resize options** - Pixel dimensions or percentage with aspect ratio control
- ⌨️ **Hotkeys** - Quick access to common actions
- 🧠 **Open source** - Built with modern C# and WPF

## 🚀 Getting Started

### Requirements

- Windows 10/11
- .NET 8.0 Runtime

### Installation

1. Download the latest release from [Releases](https://github.com/yourusername/img2go/releases)
2. Run `Img2Go.exe` (portable version) or install using the MSIX installer
3. Start converting images!

## ⌨️ Hotkeys

- `Ctrl+O` - Open files
- `Ctrl+Shift+R` - Show resize dialog
- `Ctrl+E` - Export/Convert
- `Del` - Remove selected file
- `Ctrl+L` - Toggle light/dark mode

## 🛠️ Building from Source

### Prerequisites

- Visual Studio 2022 or VS Code with C# Dev Kit
- .NET 8 SDK

### Steps

```bash
# Clone the repository
git clone https://github.com/yourusername/img2go.git
cd img2go

# Restore packages
dotnet restore

# Build
dotnet build --configuration Release

# Run
dotnet run
```

## 📦 Tech Stack

- **Language**: C#
- **Framework**: WPF (.NET 8)
- **Image Processing**: ImageSharp
- **MVVM**: CommunityToolkit.Mvvm
- **UI**: Fluent Design / Modern WPF

## 🎯 Usage

1. **Add Images**: Drag and drop files/folders or use `Ctrl+O`
2. **Configure**: Select output format, adjust quality, set resize dimensions
3. **Convert**: Click "Convert" or press `Ctrl+E`
4. **Done**: Images saved to your selected output folder

## 🔒 Privacy

- ✅ 100% offline processing
- ✅ No telemetry
- ✅ No data collection
- ✅ No internet connection required
- ✅ All settings stored locally

## 📝 License

MIT License - see LICENSE file for details

## 🤝 Contributing

Contributions welcome! Please feel free to submit a Pull Request.

## 💬 Support

For issues, feature requests, or questions, please open an issue on GitHub.

---

**Made with ❤️ for privacy-conscious users**

>>>>>>> 43e2f2e (Initial commit - Img2Go app release)
