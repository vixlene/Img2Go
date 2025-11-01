using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;

namespace Img2Go.Models
{
    public partial class ImageItem : ObservableObject
    {
        [ObservableProperty]
        private string _filePath;

        [ObservableProperty]
        private string _fileName;

        [ObservableProperty]
        private long _fileSize;

        partial void OnFilePathChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var fileInfo = new FileInfo(value);
                FileName = fileInfo.Name;
                FileSize = fileInfo.Length;
            }
        }

        public string FormattedSize
        {
            get
            {
                if (FileSize < 1024) return $"{FileSize} B";
                if (FileSize < 1024 * 1024) return $"{FileSize / 1024.0:F1} KB";
                return $"{FileSize / (1024.0 * 1024.0):F1} MB";
            }
        }
    }
}

