using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Img2Go.Controls
{
    public partial class ImagePreviewControl : UserControl
    {
        public static readonly DependencyProperty PreviewImageProperty =
            DependencyProperty.Register(nameof(PreviewImage), typeof(ImageSource), typeof(ImagePreviewControl));

        public ImageSource PreviewImage
        {
            get => (ImageSource)GetValue(PreviewImageProperty);
            set => SetValue(PreviewImageProperty, value);
        }

        private double _zoomFactor = 1.0;

        public ImagePreviewControl()
        {
            InitializeComponent();
            this.MouseWheel += ImagePreviewControl_MouseWheel;
        }

        private void ImagePreviewControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                var delta = e.Delta > 0 ? 1.1 : 0.9;
                _zoomFactor = System.Math.Max(0.1, System.Math.Min(5.0, _zoomFactor * delta));
                ApplyZoom();
                e.Handled = true;
            }
        }

        private void ApplyZoom()
        {
            if (ImageContainer != null)
            {
                var scale = new ScaleTransform(_zoomFactor, _zoomFactor);
                ImageContainer.RenderTransform = scale;
            }
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            _zoomFactor = System.Math.Min(5.0, _zoomFactor * 1.2);
            ApplyZoom();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            _zoomFactor = System.Math.Max(0.1, _zoomFactor * 0.8);
            ApplyZoom();
        }

        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            _zoomFactor = 1.0;
            ApplyZoom();
            PreviewScrollViewer.ScrollToHorizontalOffset(0);
            PreviewScrollViewer.ScrollToVerticalOffset(0);
        }
    }
}

