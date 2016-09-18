using System.ComponentModel;
using System.IO;
using System.Windows;
using SvgToXaml.Properties;
using SvgToXaml.ViewModels;

namespace SvgToXaml
{
	//todo: github oder codeplex anlegen
	//todo: Fehlerbehandlung beim Laden

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new SvgImagesViewModel();
            ((SvgImagesViewModel) DataContext).CurrentDir = Settings.Default.LastDir;
        }

       
        protected override void OnClosing(CancelEventArgs e)
        {
            //Save current Dir for next Start
            Settings.Default.LastDir = ((SvgImagesViewModel) DataContext).CurrentDir;
            Settings.Default.Save();

            base.OnClosing(e);
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var path in paths)
                {
                    if (Directory.Exists(path))
                    {
                        ((SvgImagesViewModel) DataContext).CurrentDir = path;
                    }
                    else
                    {
                        if (File.Exists(path))
                        {
                            ImageBaseViewModel.OpenDetailWindow(new SvgImageViewModel(path));
                        }
                    }
                }
            }
        }
    }

   
}
