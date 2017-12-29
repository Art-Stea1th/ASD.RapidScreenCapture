using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using D = System.Drawing;

namespace ASD.RSC.Services {

    internal sealed class ScreenCaptureService {

        // System.Drawing

        private D.Imaging.PixelFormat dFormat = D.Imaging.PixelFormat.Format32bppArgb;
        private D.Imaging.ImageLockMode dLockMode = D.Imaging.ImageLockMode.ReadOnly;
        private D.Rectangle dRect;

        // System.Windows.Media

        private PixelFormat format = PixelFormats.Bgra32;
        private int bufferSize;
        private Int32Rect rect;

        public WriteableBitmap ScreenBuffer { get; private set; }

        // Common

        private int width, height;

        public ScreenCaptureService() => ValidateScreenParameters();

        public void UpdateScreenBuffer() {

            using (var bitmap = new D.Bitmap(width, height, dFormat)) {
                using (var graphics = D.Graphics.FromImage(bitmap)) {

                    ValidateScreenParameters();
                    graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

                    var data = bitmap.LockBits(dRect, dLockMode, dFormat);
                    ScreenBuffer.WritePixels(rect, data.Scan0, bufferSize, ScreenBuffer.BackBufferStride);
                    bitmap.UnlockBits(data);
                }
            }
        }

        private void ValidateScreenParameters() {
            if (ScreenSizeUpdated()) {
                UpdateRectangles();
                ReallocateScreenBuffer();
            }
        }

        private bool ScreenSizeUpdated() {

            var newWidth = (int)SystemParameters.PrimaryScreenWidth;
            var newHeight = (int)SystemParameters.PrimaryScreenHeight;

            if (width == newWidth && height == newHeight) {
                return false;
            }
            width = newWidth; height = newHeight; return true;
        }

        private void UpdateRectangles() {
            dRect = new D.Rectangle(0, 0, width, height);
            rect = new Int32Rect(0, 0, width, height);
        }

        private void ReallocateScreenBuffer() {
            ScreenBuffer = new WriteableBitmap(width, height, 96, 96, format, null);
            bufferSize = ScreenBuffer.PixelHeight * ScreenBuffer.BackBufferStride;
        }
    }
}