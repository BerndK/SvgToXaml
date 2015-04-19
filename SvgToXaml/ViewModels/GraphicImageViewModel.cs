using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SvgToXaml.ViewModels
{
    class GraphicImageViewModel: ImageBaseViewModel
    {
        public GraphicImageViewModel(string filepath) : base(filepath)
        {
        }
        protected override ImageSource GetImageSource()
        {
            return new BitmapImage(new Uri(Filepath, UriKind.RelativeOrAbsolute));
        }

        public static string SupportedFormats { get { return "*.jpg|*.jpeg|*.png|*.bmp|*.tiff|*.gif"; } }
        protected override string GetSvgDesignInfo()
        {
            if (PreviewSource != null && PreviewSource is BitmapImage)
            {
                var bi = (BitmapImage)PreviewSource;
                return string.Format("{0}x{1}", bi.PixelWidth, bi.PixelHeight);
            }
            return null;
        }

        public override bool HasXaml { get { return false; } }
        public override bool HasSvg { get { return false; } }

    }
}
