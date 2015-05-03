using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfDemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangeBaseColor_OnClick(object sender, RoutedEventArgs e)
        {
            var baseColor = (SolidColorBrush)FindResource("ImagesColorBrush1");
            baseColor.Color = Colors.Brown;
        }

        private void ChangeSingleColor_OnClick(object sender, RoutedEventArgs e)
        {
            ((SolidColorBrush)(FindResource("JOG_BrushColor1"))).Color = Colors.SlateGray;
            ((SolidColorBrush)(FindResource("JOG_BrushColor2"))).Color = Colors.Red;
            ((SolidColorBrush)(FindResource("JOG_BrushColor4"))).Color = Colors.Yellow;
        }
    }

    public static class ColorHelper
    {
        public static void SetFromColor(this Color destColor, Color sourceColor)
        {
            destColor.R = sourceColor.R;
            destColor.G = sourceColor.G;
            destColor.B = sourceColor.B;
            destColor.A = sourceColor.A;
        }
    }
}
