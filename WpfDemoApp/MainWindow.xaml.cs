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
            var baseColor = (SolidColorBrush) FindResource("ImagesColorBrush1");
            baseColor.Color = Colors.Brown;
        }

        private void ChangeSingleColor_OnClick(object sender, RoutedEventArgs e)
        {
            ((SolidColorBrush) (FindResource("JOGBrushColor1"))).Color = Colors.SlateGray;
            ((SolidColorBrush) (FindResource("JOGBrushColor4"))).Color = Colors.Red;
            ((SolidColorBrush) (FindResource("JOGBrushColor2"))).Color = Colors.Yellow;
        }
    }
}
