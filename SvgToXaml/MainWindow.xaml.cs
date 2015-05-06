using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using SvgToXaml.Properties;
using SvgToXaml.ViewModels;

namespace SvgToXaml
{
	//todo: github oder codeplex anlegen
	//todo: Fehlerbehandlung beim Laden

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new SvgImagesViewModel();
            (DataContext as SvgImagesViewModel).CurrentDir = Settings.Default.LastDir;
        }

       
        protected override void OnClosing(CancelEventArgs e)
        {
            //Save current Dir for next Start
            Settings.Default.LastDir = (DataContext as SvgImagesViewModel).CurrentDir;
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
                        (DataContext as SvgImagesViewModel).CurrentDir = path;
                    }
                    else
                    {
                        if (File.Exists(path))
                        {
                            SvgImageViewModel.OpenDetailWindow(new SvgImageViewModel(path));
                        }
                    }
                }
            }
        }
    }

   
}
