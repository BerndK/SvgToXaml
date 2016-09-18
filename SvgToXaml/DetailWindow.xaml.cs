using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SvgToXaml
{
    /// <summary>
    /// Interaction logic for DetailWindow.xaml
    /// </summary>
    public partial class DetailWindow
    {
        public DetailWindow()
        {
            InitializeComponent();
        }

        private void CopyToClipboardClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(XmlViewer.Text);
        }

        private void ToggleStretchClicked(object sender, MouseButtonEventArgs e)
        {
            var values = Enum.GetValues(typeof(Stretch)).OfType<Stretch>().ToList();
            var idx = values.IndexOf(Image.Stretch);
            idx = (idx + 1) % values.Count;
            Image.Stretch = values[idx];
        }
    }
}
