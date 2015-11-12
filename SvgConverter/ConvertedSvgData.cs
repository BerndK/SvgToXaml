using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace SvgConverter
{
    public class ConvertedSvgData
    {
        private string _filepath;
        private string _xaml;
        private string _svg;
        private string _objectName;
        private DependencyObject _convertedObj;

        public string Filepath
        {
            get { return _filepath; }
            set { _filepath = value; }
        }

        public string Xaml
        {
            get
            {
                if (_xaml == null)
                {
                    _xaml = ConverterLogic.SvgObjectToXaml(ConvertedObj, false, _objectName);
                }
                return _xaml;
            }
            set { _xaml = value; }
        }

        public string Svg
        {
            get
            {
                if (_svg == null)
                    _svg = File.ReadAllText(_filepath);
                return _svg;
            }
            set { _svg = value; }
        }

        public DependencyObject ConvertedObj
        {
            get
            {
                if (_convertedObj == null)
                {
                    _convertedObj = ConverterLogic.ConvertSvgToObject(_filepath, ResultMode.DrawingImage, null, out _objectName, new ResKeyInfo()) as DependencyObject;
                }
                return _convertedObj;
            }
            set { _convertedObj = value; }
        }
    }
}
