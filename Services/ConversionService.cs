using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Tiff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Img2Go.Services
{
    public class ConversionService
    {
        public event EventHandler<ConversionProgressEventArgs> ProgressChanged;

        public async Task<ConversionResult> ConvertImageAsync(
            string inputPath,
            string outputPath,
            ImageFormat targetFormat,
            int? quality = null,
            int? maxWidth = null,
            int? maxHeight = null,
            bool maintainAspectRatio = true,
            Rectangle? cropRect = null,
            CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var image = Image.Load(inputPath);
                    var originalSize = image.Size();

                    // Apply crop if specified
                    if (cropRect.HasValue)
                    {
                        image.Mutate(x => x.Crop(cropRect.Value));
                    }

                    // Apply resize if specified
                    if (maxWidth.HasValue || maxHeight.HasValue)
                    {
                        var options = new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(maxWidth ?? int.MaxValue, maxHeight ?? int.MaxValue)
                        };

                        if (!maintainAspectRatio && maxWidth.HasValue && maxHeight.HasValue)
                        {
                            options.Mode = ResizeMode.Crop;
                            options.Size = new Size(maxWidth.Value, maxHeight.Value);
                        }

                        image.Mutate(x => x.Resize(options));
                    }

                    // Get encoder based on format
                    IImageEncoder encoder = GetEncoder(targetFormat, quality ?? 90);

                    // Ensure output directory exists
                    var outputDir = Path.GetDirectoryName(outputPath);
                    if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }

                    // Save image
                    image.Save(outputPath, encoder);

                    var finalSize = new FileInfo(outputPath).Length;
                    var originalFileSize = new FileInfo(inputPath).Length;

                    return new ConversionResult
                    {
                        Success = true,
                        InputPath = inputPath,
                        OutputPath = outputPath,
                        OriginalSize = originalFileSize,
                        FinalSize = finalSize,
                        OriginalDimensions = originalSize,
                        FinalDimensions = image.Size()
                    };
                }
                catch (Exception ex)
                {
                    return new ConversionResult
                    {
                        Success = false,
                        InputPath = inputPath,
                        OutputPath = outputPath,
                        ErrorMessage = ex.Message
                    };
                }
            }, cancellationToken);
        }

        public async Task<List<ConversionResult>> ConvertBatchAsync(
            List<string> inputPaths,
            string outputDirectory,
            ImageFormat targetFormat,
            int? quality = null,
            int? maxWidth = null,
            int? maxHeight = null,
            bool maintainAspectRatio = true,
            Rectangle? cropRect = null,
            CancellationToken cancellationToken = default)
        {
            var results = new List<ConversionResult>();
            var total = inputPaths.Count;
            var completed = 0;

            // Process in parallel with degree of parallelism
            var semaphore = new SemaphoreSlim(Environment.ProcessorCount * 2);
            var tasks = inputPaths.Select(async inputPath =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(inputPath);
                    var extension = GetExtension(targetFormat);
                    var outputPath = Path.Combine(outputDirectory, $"{fileName}{extension}");

                    var result = await ConvertImageAsync(
                        inputPath,
                        outputPath,
                        targetFormat,
                        quality,
                        maxWidth,
                        maxHeight,
                        maintainAspectRatio,
                        cropRect,
                        cancellationToken);

                    lock (results)
                    {
                        results.Add(result);
                        completed++;
                        ProgressChanged?.Invoke(this, new ConversionProgressEventArgs
                        {
                            Completed = completed,
                            Total = total,
                            CurrentFile = inputPath
                        });
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
            return results;
        }

        private IImageEncoder GetEncoder(ImageFormat format, int quality)
        {
            return format switch
            {
                ImageFormat.Jpeg => new JpegEncoder { Quality = quality },
                ImageFormat.Png => new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression },
                ImageFormat.WebP => new WebpEncoder { Quality = quality },
                ImageFormat.Bmp => new BmpEncoder(),
                ImageFormat.Tiff => new TiffEncoder { CompressionLevel = TiffCompressionLevel.Lzw },
                _ => new JpegEncoder { Quality = quality }
            };
        }

        private string GetExtension(ImageFormat format)
        {
            return format switch
            {
                ImageFormat.Jpeg => ".jpg",
                ImageFormat.Png => ".png",
                ImageFormat.WebP => ".webp",
                ImageFormat.Bmp => ".bmp",
                ImageFormat.Tiff => ".tiff",
                _ => ".jpg"
            };
        }

        public static ImageFormat GetFormatFromExtension(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => ImageFormat.Jpeg,
                ".png" => ImageFormat.Png,
                ".webp" => ImageFormat.WebP,
                ".bmp" => ImageFormat.Bmp,
                ".tiff" or ".tif" => ImageFormat.Tiff,
                ".heic" or ".heif" => ImageFormat.Jpeg, // Convert HEIC to JPEG
                _ => ImageFormat.Jpeg
            };
        }

        public static bool IsSupportedImageFormat(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            var supported = new[] { ".jpg", ".jpeg", ".png", ".webp", ".bmp", ".tiff", ".tif", ".heic", ".heif" };
            return supported.Contains(extension);
        }
    }

    public enum ImageFormat
    {
        Jpeg,
        Png,
        WebP,
        Bmp,
        Tiff
    }

    public class ConversionResult
    {
        public bool Success { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public long OriginalSize { get; set; }
        public long FinalSize { get; set; }
        public Size OriginalDimensions { get; set; }
        public Size FinalDimensions { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ConversionProgressEventArgs : EventArgs
    {
        public int Completed { get; set; }
        public int Total { get; set; }
        public string CurrentFile { get; set; }
    }
}

