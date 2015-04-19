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
	//todo: git anlegen
    //todo: Baum beim Start ausklappen
    //todo: Datail Contextmenu Anwendung starten (Inkscape)
    //todo: Baum Contextmenu Ordner öffnen
    //todo: Png, bmp, jpg Dateien anzeigen
	//todo: Ausgabe von Xaml für Ordner ermöglichen
	//todo: github oder codeplex anlegen
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new SvgImages();
            (DataContext as SvgImages).CurrentDir = Settings.Default.LastDir;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.FolderTree.CurrentFolder = @"D:\Projects\Logging\Logging\LogInspector\Gui\Images\Svg";
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //Save current Dir for next Start
            Settings.Default.LastDir = (DataContext as SvgImages).CurrentDir;
            Settings.Default.Save();

            base.OnClosing(e);
        }

        private void MainWindow_OnDropp(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var path in paths)
                {
                    if (Directory.Exists(path))
                    {
                        (DataContext as SvgImages).CurrentDir = path;
                    }
                    else
                    {
                        if (File.Exists(path))
                        {
                            SvgConvertedImage.OpenDetailWindow(new SvgConvertedImage((DataContext as SvgImages), path));
                        }
                    }
                }
            }
        }
    }

   
}
