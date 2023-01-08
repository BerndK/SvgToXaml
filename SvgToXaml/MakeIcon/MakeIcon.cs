using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
using System.Drawing.Imaging;
using System.IO;

namespace SvgToXaml
{
    public static class MakeIcon
    {
        public static Icon ToIcon(this ImageSource imageSource, int size = 64)
        {
            if (imageSource == null) return null;

            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = imageSource;
            image.Arrange(new Rect(0, 0, size, size));
            image.UpdateLayout();

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(image);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            MemoryStream stream = new MemoryStream();
            encoder.Save(stream);

            Bitmap bmp = new Bitmap(stream);
            IntPtr Hicon = bmp.GetHicon();
            Icon newIcon = Icon.FromHandle(Hicon);

            return newIcon;
        }
    }
}
