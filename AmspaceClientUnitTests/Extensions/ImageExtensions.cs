using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;

namespace AmspaceClientUnitTests.Extensions
{
    public static class ImageExtensions
    {
        public static MemoryStream CreateRandomBitmap()
        {
            PixelFormat pf = PixelFormats.Bgr32;
            int width = 200;
            int height = 200;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * height];
            Random value = new Random();
            value.NextBytes(rawImage);
            var source = BitmapSource.Create(
                width, height,
                96, 96, pf, null,
                rawImage, rawStride);

            var bmp = new MemoryStream();

            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(source));
            enc.Save(bmp);

            return bmp;
        }
    }
}
