using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SvgToXaml
{
    /// <summary>
    /// Interaction logic for DetailWindow.xaml
    /// </summary>
    public partial class DetailWindow
    {
        private static readonly KeyValuePair<int, string>[] iconsSizes = {
            new KeyValuePair<int, string>(16, "16 x 16"),
            new KeyValuePair<int, string>(32, "32 x 32"),
            new KeyValuePair<int, string>(64, "64 x 64"),
            new KeyValuePair<int, string>(128, "128 x 128"),
            new KeyValuePair<int, string>(256, "256 x 256"),
            new KeyValuePair<int, string>(512, "512 x 512"),
            new KeyValuePair<int, string>(1024, "1024 x 1024")
        };

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

        private void btnGenerateIcon_Click(object sender, RoutedEventArgs e)
        {
            int sizeOfIcon = int.Parse(((System.Windows.FrameworkElement)this.cbIconSize.SelectedItem).Tag.ToString());
            Icon icon = MakeIcon.ToIcon(this.Image.Source,sizeOfIcon);
            SaveFileDialog fd = new SaveFileDialog() { Title = "Escolha o arquivo de icone", AddExtension= true, Filter = "Ico Files (*.ico)|*.ico", OverwritePrompt = true, ValidateNames = true };
            if (fd.ShowDialog()==true)
                using (FileStream fs = new FileStream(fd.FileName, FileMode.Create)){
                    icon.Save(fs);
                }

        }
    }
}
