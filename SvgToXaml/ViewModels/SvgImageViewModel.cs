using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using LocalizationControl.Command;
using SvgConverter;

namespace SvgToXaml.ViewModels
{
    public class SvgImageViewModel : ImageBaseViewModel
    {
        private ConvertedSvgData _convertedSvgData;


        public SvgImageViewModel(string filepath) : base(filepath)
        {
        }

        public SvgImageViewModel(ConvertedSvgData convertedSvgData)
            : this(convertedSvgData.Filepath)
        {
            _convertedSvgData = convertedSvgData;
        }

        public static SvgImageViewModel DesignInstance
        {
            get
            {
                var imageSource = new DrawingImage(new GeometryDrawing(Brushes.Black, null, new RectangleGeometry(new Rect(new Size(10, 10)), 1, 1)));
                var data = new ConvertedSvgData { ConvertedObj = imageSource, Filepath = "FilePath", Svg = "<svg/>", Xaml = "<xaml/>" };
                return new SvgImageViewModel(data);
            }
        }

        protected override ImageSource GetImageSource()
        {
            return SvgData != null ? SvgData.ConvertedObj as ImageSource : null;
        }

        protected override string GetSvgDesignInfo()
        {
            if (PreviewSource != null && PreviewSource is DrawingImage)
            {
                var di = (DrawingImage) PreviewSource;
                if (di.Drawing is DrawingGroup)
                {
                    var dg = (DrawingGroup) di.Drawing;
                    var bounds = (dg.ClipGeometry != null) ? dg.ClipGeometry.Bounds : dg.Bounds;
                    return string.Format("{0:#.##}x{1:#.##}", bounds.Width, bounds.Height);
                }
            }
            return null;
        }

        public override bool HasXaml { get { return true; } }
        public override bool HasSvg { get { return true; } }

        public string Svg { get { return SvgData != null ? SvgData.Svg : null; } }

        public string Xaml { get { return SvgData != null ? SvgData.Xaml : null; } }


        public ConvertedSvgData SvgData
        {
            get
            {
                if (_convertedSvgData == null)
                {
                    try
                    {
                        _convertedSvgData = ConverterLogic.ConvertSvg(_filepath, ResultMode.DrawingImage);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                    
                    //verzögertes Laden: ist scheiß lahm
                    //InUi(DispatcherPriority.Loaded, () =>
                    //{
                    //    _convertedSvgData = ConverterLogic.ConvertSvg(_filepath, ResultMode.DrawingImage);
                    //    OnPropertyChanged("");
                    //});
                    //return null;
                }
                return _convertedSvgData;
            }
        }
    }
}
