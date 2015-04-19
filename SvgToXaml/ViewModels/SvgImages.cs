using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using LocalizationControl.Command;
using SvgConverter;
using SvgToXaml.Infrastructure;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace SvgToXaml.ViewModels
{
    public class SvgImages : ViewModelBase
    {
        private string _currentDir;
        private ObservableCollectionSafe<ImageBaseViewModel> _images;
        private ImageBaseViewModel _selectedItem;

        public SvgImages()
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
            string outFileName = CurrentDir + Path.GetFileNameWithoutExtension(CurrentDir) + ".xaml"; 
            var saveDlg = new SaveFileDialog {AddExtension = true, DefaultExt = ".xaml", Filter = "Xaml-File|*.xaml", FileName = outFileName};
            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                outFileName = saveDlg.FileName;
                File.WriteAllText(outFileName, ConverterLogic.SvgDirToXaml(CurrentDir, Path.GetFileNameWithoutExtension(outFileName)));
                MessageBox.Show(outFileName + "\nhas been written");
            }
        }

        private void InfoExecute()
        {
            MessageBox.Show("SvgToXaml © 2015 Bernd Klaiber\n\nPowered by sharpvectors.codeplex.com", "Info");
        }
        private void OpenExplorerExecute(string path)
        {
            Process.Start(path);
        }

        public static SvgImages DesignInstance
        {
            get
            {
                var result = new SvgImages();
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
            return filters.Split('|').SelectMany(filter => System.IO.Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
        }
    }
}
