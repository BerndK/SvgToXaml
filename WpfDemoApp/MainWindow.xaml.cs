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
using RelativeBrushes;

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
            //var brush = Props.GetContentBrush(Image1) as SolidColorBrush;
//            brush.Color = Colors.Yellow;
        }

        private void ChangeBaseColor_OnClick(object sender, RoutedEventArgs e)
        {
            //var baseColor = (SolidColorBrush) FindResource("Color1Brush");
            //baseColor.Color = Colors.Brown;
            //var baseColorC = (SolidColorBrush)FindResource(ImagesC.ImagesC.Color1BrushKey);
            //baseColorC.Color = Colors.Brown;

            Props.SetContentBrush(Image1, Brushes.Yellow);
        }

        private void ChangeSingleColor_OnClick(object sender, RoutedEventArgs e)
        {
            //((SolidColorBrush) (FindResource("JOGColor1Brush"))).Color = Colors.SlateGray;
            //((SolidColorBrush) (FindResource("JOGColor4Brush"))).Color = Colors.Red;
            //((SolidColorBrush) (FindResource("JOGColor2Brush"))).Color = Colors.Yellow;
            BrushCollection[3] = Brushes.Orange;
        }

        private void BtnChangeBrushesColor_OnClick(object sender, RoutedEventArgs e)
        {
            var brushes = FindResource("BrushCollectionRes") as BrushCollection;
            brushes[3] = Brushes.Orange;
        }
    }
}
