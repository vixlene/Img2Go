using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Img2Go.Models;
using Img2Go.Services;
using SixLabors.ImageSharp;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Img2Go.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ConversionService _conversionService;
        private CancellationTokenSource _cancellationTokenSource;

        [ObservableProperty]
        private ObservableCollection<ImageItem> _imageItems = new();

        [ObservableProperty]
        private ImageItem _selectedImage;

        [ObservableProperty]
        private bool _isProcessing;

        [ObservableProperty]
        private double _progress;

        [ObservableProperty]
        private string _statusMessage = "Ready";

        [ObservableProperty]
        private string _selectedFormat = "Jpeg";

        [ObservableProperty]
        private int _quality = 85;

        [ObservableProperty]
        private bool _maintainAspectRatio = true;

        [ObservableProperty]
        private int? _resizeWidth;

        [ObservableProperty]
        private int? _resizeHeight;

        [ObservableProperty]
        private Rectangle? _cropRectangle;

        [ObservableProperty]
        private BitmapImage _previewImage;

        [ObservableProperty]
        private bool _showCropOverlay;

        private string _outputDirectory;

        public MainViewModel()
        {
            _conversionService = ServiceLocator.ConversionService;
            _conversionService.ProgressChanged += OnConversionProgressChanged;

            LoadSettings();
        }

        partial void OnSelectedImageChanged(ImageItem value)
        {
            if (value != null)
            {
                LoadPreview(value.FilePath);
            }
            else
            {
                PreviewImage = null;
            }
        }

        [RelayCommand]
        private void OpenFiles()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.webp;*.bmp;*.tiff;*.tif;*.heic;*.heif|All Files|*.*",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                AddFiles(dialog.FileNames);
            }
        }

        [RelayCommand]
        private void OpenFolder()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var files = Directory.GetFiles(dialog.SelectedPath)
                    .Where(f => ConversionService.IsSupportedImageFormat(f))
                    .ToArray();
                AddFiles(files);
            }
        }

        [RelayCommand]
        private void RemoveSelected()
        {
            if (SelectedImage != null)
            {
                ImageItems.Remove(SelectedImage);
                if (ImageItems.Count > 0)
                {
                    SelectedImage = ImageItems[0];
                }
                else
                {
                    SelectedImage = null;
                }
            }
        }

        [RelayCommand]
        private async Task Convert()
        {
            if (ImageItems.Count == 0) return;

            IsProcessing = true;
            Progress = 0;
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var outputDir = GetOutputDirectory();
                if (string.IsNullOrEmpty(outputDir)) return;

                var format = Enum.Parse<ImageFormat>(SelectedFormat);
                var startTime = DateTime.Now;

                var paths = ImageItems.Select(i => i.FilePath).ToList();
                var results = await _conversionService.ConvertBatchAsync(
                    paths,
                    outputDir,
                    format,
                    Quality,
                    ResizeWidth,
                    ResizeHeight,
                    MaintainAspectRatio,
                    CropRectangle,
                    _cancellationTokenSource.Token);

                var elapsed = (DateTime.Now - startTime).TotalSeconds;
                var successCount = results.Count(r => r.Success);

                StatusMessage = $"Converted {successCount} images in {elapsed:F1} seconds ⚡";
                Progress = 100;

                // Show summary
                MessageBox.Show(
                    $"Conversion complete!\n\n" +
                    $"Success: {successCount}/{results.Count}\n" +
                    $"Time: {elapsed:F1}s\n" +
                    $"Output: {outputDir}",
                    "Conversion Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (OperationCanceledException)
            {
                StatusMessage = "Conversion cancelled";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        private void CancelConversion()
        {
            _cancellationTokenSource?.Cancel();
        }

        [RelayCommand]
        private void ShowResizeDialog()
        {
            // Resize dialog would be shown here
            // For now, just toggle visibility of resize controls
        }

        [RelayCommand]
        private void ToggleTheme()
        {
            ThemeService.Instance.ToggleTheme();
        }

        public void AddFiles(string[] filePaths)
        {
            foreach (var filePath in filePaths)
            {
                if (ConversionService.IsSupportedImageFormat(filePath))
                {
                    if (!ImageItems.Any(i => i.FilePath == filePath))
                    {
                        ImageItems.Add(new ImageItem { FilePath = filePath });
                    }
                }
            }

            if (SelectedImage == null && ImageItems.Count > 0)
            {
                SelectedImage = ImageItems[0];
            }
        }

        public void HandleDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var imageFiles = files.Where(f => File.Exists(f) && ConversionService.IsSupportedImageFormat(f)).ToArray();
                AddFiles(imageFiles);

                var folders = files.Where(f => Directory.Exists(f)).ToArray();
                foreach (var folder in folders)
                {
                    var folderFiles = Directory.GetFiles(folder, "*", SearchOption.AllDirectories)
                        .Where(f => ConversionService.IsSupportedImageFormat(f))
                        .ToArray();
                    AddFiles(folderFiles);
                }
            }
        }

        private void LoadPreview(string filePath)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(filePath);
                bitmap.EndInit();
                bitmap.Freeze();

                PreviewImage = bitmap;
            }
            catch
            {
                PreviewImage = null;
            }
        }

        private string GetOutputDirectory()
        {
            var settings = SettingsService.Instance.Settings;
            var defaultDir = string.IsNullOrEmpty(settings.LastOutputDirectory)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Img2Go")
                : settings.LastOutputDirectory;

            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                SelectedPath = defaultDir
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _outputDirectory = dialog.SelectedPath;
                settings.LastOutputDirectory = _outputDirectory;
                SettingsService.Instance.SaveSettings();
                return _outputDirectory;
            }

            return null;
        }

        private void OnConversionProgressChanged(object sender, ConversionProgressEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Progress = (double)e.Completed / e.Total * 100;
                StatusMessage = $"Processing {e.Completed}/{e.Total}: {Path.GetFileName(e.CurrentFile)}";
            });
        }

        private void LoadSettings()
        {
            var settings = SettingsService.Instance.Settings;
            Quality = settings.DefaultQuality;
            MaintainAspectRatio = settings.MaintainAspectRatio;
            SelectedFormat = settings.DefaultOutputFormat;
        }
    }
}

