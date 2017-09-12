using System.Windows;
using System.Windows.Media;
using RelativeBrushes;

namespace WpfDemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangeColor_OnClick(object sender, RoutedEventArgs e)
        {
            Props.SetContentBrush(ButtonImage1, Brushes.Yellow);
            Button2.Foreground = Brushes.Yellow;
        }

        private void ChangeMiddleColor_OnClick(object sender, RoutedEventArgs e)
        {
            BrushCollection[2] = Brushes.Green;
        }

        private void BtnChangeMiddleColors_OnClick(object sender, RoutedEventArgs e)
        {
            //Many icons have same Color (application wide)
            var brushes = FindResource("BrushCollectionRes") as BrushCollection;
            if (brushes != null)
                brushes[2] = Brushes.Green;
        }
    }
}
