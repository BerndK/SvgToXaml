using System;
using System.Collections.Generic;
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
    public class SvgConvertedImage : ViewModelBase
    {
        private readonly SvgImages _parent;
        private ConvertedSvgData _convertedSvgData;
        private string _filepath;


        public SvgConvertedImage(SvgImages parent, string filepath)
        {
            _parent = parent;
            _filepath = filepath;
            OpenDetailCommand = new DelegateCommand(OpenDetailExecute);
        }

        public SvgConvertedImage(SvgImages parent, ConvertedSvgData convertedSvgData)
            : this(parent, convertedSvgData.Filepath)
        {
            _convertedSvgData = convertedSvgData;
        }

        public static SvgConvertedImage DesignInstance
        {
            get
            {
                var imageSource = new DrawingImage(new GeometryDrawing(Brushes.Black, null, new RectangleGeometry(new Rect(new Size(10, 10)), 1, 1)));
                var data = new ConvertedSvgData { ConvertedObj = imageSource, Filepath = "FilePath", Svg = "<svg/>", Xaml = "<xaml/>" };
                return new SvgConvertedImage(null, data);
            }
        }

        public string Filepath { get { return _filepath; } }

        public string Filename { get { return Path.GetFileName(_filepath); } }

        public ImageSource PreviewSource { get { return SvgData != null ? SvgData.ConvertedObj as ImageSource : null; } }

        public string SvgDesignInfo
        {
            get
            {
                if (PreviewSource != null && PreviewSource is DrawingImage)
                {
                    var di = (DrawingImage)PreviewSource;
                    if (di.Drawing is DrawingGroup)
                    {
                        var dg = (DrawingGroup)di.Drawing;
                        var bounds = (dg.ClipGeometry != null) ? dg.ClipGeometry.Bounds : dg.Bounds;
                        return string.Format("{0}x{1}", bounds.Width, bounds.Height);
                    }
                }
                return null;
            }
        }

        public string Svg { get { return SvgData != null ? SvgData.Svg : null; } }

        public string Xaml { get { return SvgData != null ? SvgData.Xaml : null; } }

        public ICommand OpenDetailCommand { get; set; }

        public ConvertedSvgData SvgData
        {
            get
            {
                if (_convertedSvgData == null)
                {
                    _convertedSvgData = ConverterLogic.ConvertSvg(_filepath, ResultMode.DrawingImage);
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

        private void OpenDetailExecute()
        {
            OpenDetailWindow(this);
        }

        public static void OpenDetailWindow(SvgConvertedImage convertedImage)
        {
            new DetailWindow { DataContext = convertedImage }.Show();
        }
    }
}
