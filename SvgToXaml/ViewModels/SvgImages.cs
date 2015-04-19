using System;
using System.Collections.Generic;
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
        private ObservableCollectionSafe<SvgConvertedImage> _images;
        private SvgConvertedImage _selectedItem;

        public SvgImages()
        {
            _images = new ObservableCollectionSafe<SvgConvertedImage>();
            OpenFileCommand = new DelegateCommand(OpenFileExecute);
            OpenFolderCommand = new DelegateCommand(OpenFolderExecute);
            InfoCommand = new DelegateCommand(InfoExecute);
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
                SvgConvertedImage.OpenDetailWindow(new SvgConvertedImage(this, openDlg.FileName));
            }
        }

        private void InfoExecute()
        {
            MessageBox.Show("SvgToXaml © 2015 Bernd Klaiber\n\nPowered by sharpvectors.codeplex.com", "Info");
        }

        public static SvgImages DesignInstance
        {
            get
            {
                var result = new SvgImages();
                result.Images.Add(SvgConvertedImage.DesignInstance);
                result.Images.Add(SvgConvertedImage.DesignInstance);
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

        public SvgConvertedImage SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        public ObservableCollectionSafe<SvgConvertedImage> Images
        {
            get { return _images; }
            set { SetProperty(ref _images, value); }
        }

        public ICommand OpenFolderCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand InfoCommand { get; set; }

        private void ReadImagesFromDir(string folder)
        {
            Images.Clear();
            var files = ConverterLogic.SvgFilesFromFolder(folder);
            //foreach (var file in files)
            //{
            //    var convertedSvgData = ConverterLogic.ConvertSvg(file, ResultMode.DrawingImage);
            //    Images.Add(new SvgConvertedImage(this, convertedSvgData));
            //}
            Images.AddRange(files.Select(f => new SvgConvertedImage(this, f)));
        }
    }
}
