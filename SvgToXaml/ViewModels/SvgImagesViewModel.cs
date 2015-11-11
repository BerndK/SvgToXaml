using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using LocalizationControl.Command;
using SvgConverter;
using SvgToXaml.Infrastructure;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace SvgToXaml.ViewModels
{
    public class SvgImagesViewModel : ViewModelBase
    {
        private string _currentDir;
        private ObservableCollectionSafe<ImageBaseViewModel> _images;
        private ImageBaseViewModel _selectedItem;

        public SvgImagesViewModel()
        {
            _images = new ObservableCollectionSafe<ImageBaseViewModel>();
            OpenFileCommand = new DelegateCommand(OpenFileExecute);
            OpenFolderCommand = new DelegateCommand(OpenFolderExecute);
            ExportDirCommand = new DelegateCommand(ExportDirExecute);
            InfoCommand = new DelegateCommand(InfoExecute);

            ContextMenuCommands = new ObservableCollection<Tuple<object, ICommand>>();
            ContextMenuCommands.Add(new Tuple<object, ICommand>("Open Explorer", new DelegateCommand<string>(OpenExplorerExecute))); 
        }

        private void OpenFolderExecute()
        {
            var folderDialog = new FolderBrowserDialog { Description = "Open Folder", SelectedPath = CurrentDir, ShowNewFolderButton = false };
            if (folderDialog.ShowDialog() == DialogResult.OK)
                CurrentDir = folderDialog.SelectedPath;
        }

        private void OpenFileExecute()
        {
            var openDlg = new OpenFileDialog { CheckFileExists = true, Filter = "Svg-Files|*.svg*", Multiselect = false };
            if (openDlg.ShowDialog().GetValueOrDefault())
            {
                SvgImageViewModel.OpenDetailWindow(new SvgImageViewModel(openDlg.FileName));
            }
        }

        private void ExportDirExecute()
        {
            string outFileName = Path.GetFileNameWithoutExtension(CurrentDir) + ".xaml"; 
            var saveDlg = new SaveFileDialog {AddExtension = true, DefaultExt = ".xaml", Filter = "Xaml-File|*.xaml", InitialDirectory = CurrentDir, FileName = outFileName};
            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                var namePrefix = Microsoft.VisualBasic.Interaction.InputBox("Enter a namePrefix (or leave empty to not use it)", "Name Prefix");
                if (string.IsNullOrWhiteSpace(namePrefix))
                    namePrefix = null;
                outFileName = Path.GetFullPath(saveDlg.FileName);
                File.WriteAllText(outFileName, ConverterLogic.SvgDirToXaml(CurrentDir, Path.GetFileNameWithoutExtension(outFileName), namePrefix));
                if (MessageBox.Show(outFileName + "\nhas been written\nCreate a BatchFile to automate next time?", 
                    null, MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                {
                    var outputname = Path.GetFileNameWithoutExtension(outFileName);
                    var outputdir = Path.GetDirectoryName(outFileName);
                    var batchText = string.Format("SvgToXaml BuildDict /inputdir \"{0}\" /outputdir \"{1}\" /outputname {2}", CurrentDir, outputdir, outputname);

                    if (!string.IsNullOrWhiteSpace(namePrefix))
                    {
                        batchText += " /nameprefix \"" + namePrefix + "\"";
                    }

                    batchText += "\r\npause";

                    File.WriteAllText(Path.Combine(CurrentDir, "Update.cmd"), batchText);

                    //Copy ExeFile
                    var srcFile = Environment.GetCommandLineArgs().First();
                    var destFile = Path.Combine(CurrentDir, Path.GetFileName(srcFile));
                    Console.WriteLine("srcFile:", srcFile);
                    Console.WriteLine("destFile:", destFile);
                    if (!string.Equals(srcFile, destFile, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Copying file...");
                        File.Copy(srcFile, destFile, true);
                    }
                }
            }
        }

        private void InfoExecute()
        {
            MessageBox.Show("SvgToXaml © 2015 Bernd Klaiber\n\nPowered by\nsharpvectors.codeplex.com (Svg-Support),\nicsharpcode (AvalonEdit)", "Info");
        }
        private void OpenExplorerExecute(string path)
        {
            Process.Start(path);
        }

        public static SvgImagesViewModel DesignInstance
        {
            get
            {
                var result = new SvgImagesViewModel();
                result.Images.Add(SvgImageViewModel.DesignInstance);
                result.Images.Add(SvgImageViewModel.DesignInstance);
                return result;
            }
        }

        public string CurrentDir
        {
            get { return _currentDir; }
            set
            {
                if (SetProperty(ref _currentDir, value))
                    ReadImagesFromDir(_currentDir);
            }
        }

        public ImageBaseViewModel SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        public ObservableCollectionSafe<ImageBaseViewModel> Images
        {
            get { return _images; }
            set { SetProperty(ref _images, value); }
        }

        public ICommand OpenFolderCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand ExportDirCommand { get; set; }
        public ICommand InfoCommand { get; set; }

        public ObservableCollection<Tuple<object, ICommand>> ContextMenuCommands { get; set; }

        private void ReadImagesFromDir(string folder)
        {
            Images.Clear();
            var svgFiles = ConverterLogic.SvgFilesFromFolder(folder);
            var svgImages = svgFiles.Select(f => new SvgImageViewModel(f));

            var graphicFiles = GetFilesMulti(folder, GraphicImageViewModel.SupportedFormats);
            var graphicImages = graphicFiles.Select(f => new GraphicImageViewModel(f));
            
            var allImages = svgImages.Concat<ImageBaseViewModel>(graphicImages).OrderBy(e=>e.Filepath);
            
            Images.AddRange(allImages);
        }

        private static string[] GetFilesMulti(string sourceFolder, string filters, System.IO.SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            try
            {
                if (!Directory.Exists(sourceFolder))
                    return new string[0];
                return filters.Split('|').SelectMany(filter => Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
            }
            catch (Exception)
            {
                return new string[0];
            }
        }
    }
}
