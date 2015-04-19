using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SvgConverter
{
    public class ConvertedSvgData
    {
        public string Filepath { get; set; }
        public string Xaml { get; set; }
        public string Svg { get; set; }
        public DependencyObject ConvertedObj { get; set; }
    }
}
