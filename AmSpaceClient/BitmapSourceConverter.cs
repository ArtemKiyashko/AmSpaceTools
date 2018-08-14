using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AmSpaceClient
{
    public interface IImageConverter
    {
        BitmapSource ConvertFromByteArray(byte[] array);
    }

    public class BitmapSourceConverter : IImageConverter
    {
        public BitmapSource ConvertFromByteArray(byte[] array)
        {
            return (BitmapSource)new ImageSourceConverter().ConvertFrom(array);
        }
    }
}
