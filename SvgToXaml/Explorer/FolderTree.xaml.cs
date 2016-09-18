using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SvgToXaml.Explorer
{
    /// <summary>
    ///     Interaction logic for FolderTree.xaml
    /// </summary>
    public partial class FolderTree
    {
        public static readonly DependencyProperty CurrentFolderProperty = DependencyProperty.Register(
            "CurrentFolder", typeof (string), typeof (FolderTree), new PropertyMetadata(default(string), CurrentFolderChanged));

        private static void CurrentFolderChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            FolderTree folderTree = (FolderTree)dependencyObject;
            var item =  folderTree.FindItem(folderTree.FoldersTree, (string)dependencyPropertyChangedEventArgs.NewValue);
            if (item!=null)
                folderTree.SelectItem(item);
        }

        public string CurrentFolder
        {
            get { return (string) GetValue(CurrentFolderProperty); }
            set { SetValue(CurrentFolderProperty, value); }
        }
        private readonly object _dummyNode = null;

        public FolderTree()
        {
            InitializeComponent();
            FillRootLevel();
            FoldersTree.SelectedItemChanged += FoldersTreeOnSelectedItemChanged;
        }

        public static readonly DependencyProperty ContextMenuCommandsProperty = DependencyProperty.Register(
            "ContextMenuCommands", typeof (ObservableCollection<Tuple<object, ICommand>>), typeof (FolderTree), new PropertyMetadata(default(ObservableCollection<Tuple<object, ICommand>>)));

        public ObservableCollection<Tuple<object, ICommand>> ContextMenuCommands
        {
            get { return (ObservableCollection<Tuple<object, ICommand>>) GetValue(ContextMenuCommandsProperty); }
            set { SetValue(ContextMenuCommandsProperty, value); }
        }

        private void FoldersTreeOnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            if (routedPropertyChangedEventArgs.NewValue is TreeViewItem)
                CurrentFolder = (string)((routedPropertyChangedEventArgs.NewValue as TreeViewItem).Tag);
        }

        private void FillRootLevel()
        {
            foreach (var drive in Directory.GetLogicalDrives())
            {
                var item = new TreeViewItem();
                item.Header = drive;
                item.Tag = drive;
                item.Items.Add(_dummyNode);
                item.Expanded += folder_Expanded;

                // Apply the attached property so that 
                // the triggers know that this is root item.
                TreeViewItemProps.SetIsRootLevel(item, true);

                FoldersTree.Items.Add(item);
            }
        }

        private void folder_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem) sender;
            if (item.Items.Count == 1 && item.Items[0] == _dummyNode)
            {
                item.Items.Clear();
                try
                {
                    if (item.Tag != null)
                        foreach (var dir in Directory.GetDirectories((string) item.Tag))
                        {
                            var subitem = new TreeViewItem();
                            subitem.Header = new DirectoryInfo(dir).Name;
                            subitem.Tag = dir;
                            subitem.Items.Add(_dummyNode);
                            subitem.Expanded += folder_Expanded;
                            item.Items.Add(subitem);
                        }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public void SelectItem(TreeViewItem item)
        {
            item.Focus();
            item.IsSelected = true;
            item.BringIntoView();
        }

        public TreeViewItem FindItem(TreeView tv, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;
            var parts = GetDirParts(path);
            var currItems = tv.Items;
            TreeViewItem found = null;
            foreach (var part in parts)
            {
                var newFound = currItems.Cast<TreeViewItem>()
                    .FirstOrDefault(
                        e => string.Equals(e.Header.ToString(), part, StringComparison.InvariantCultureIgnoreCase));
                if (newFound != null)
                {
                    found = newFound;
                    found.IsExpanded = true;
                    currItems = found.Items;
                }
            }
            return found;
        }

        private IEnumerable<string> GetDirParts(string path)
        {
            path = Path.GetFullPath(path);
            //yield return Path.GetPathRoot(path);
            var parts = path.Split(Path.DirectorySeparatorChar).ToArray();
            if (parts.Length > 0 && parts[0].Length == 2)
                parts[0] += @"\";
            return parts;
        }

        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged
        {
            add
            {
                FoldersTree.SelectedItemChanged += value;
            }
            remove
            {
                FoldersTree.SelectedItemChanged -= value;
            }
        }


    }
}