using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Security.AccessControl;

using BackuperLibrary.Generic;
using System.Windows.Controls;

namespace BackuperUI.Windows {
    /// <summary>
    /// Window to search a path to an existing file/directory.
    /// </summary>
    public partial class OpenPathDialog : Window {

        /// <summary>
        /// Opens a <see cref="OpenPathDialog"/> window to select an existing path.
        /// </summary>
        /// <returns>The path chosen by the user.</returns>
        public static new string Show() {
            var dialog = new OpenPathDialog();
            if(dialog.ShowDialog() == true) {
                return dialog.FullPath;
            } else {
                return null;
            }
        }

        //todo - fix bugs related to edited paths
        private DirectoryInfo CurrDir => new DirectoryInfo(PathDisplayTextBox.Text);
        private DirectoryInfo FullDir => new DirectoryInfo(FullPath);
        private string FullPath => Path.Combine(PathDisplayTextBox.Text, SelectedFolderNameTextBox.Text);

        private static DirectoryInfo DefaultPath => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

        /// <summary>
        /// <see cref="OpenPathDialog"/> window's constructor.
        /// </summary>
        private OpenPathDialog() {
            InitializeComponent();
            LoadFolder(DefaultPath);

            //get all drives(i.e: C:\, D:\, etc)
            var drives = DriveInfo.GetDrives();

            //keep only active drives
            var roots = drives.Where(x => x.IsReady).Select(x => x.RootDirectory);

            RootsDataGrid.ItemsSource = roots;
        }

        /// <summary>
        /// Gets all children—both directories and files—of <see cref="CurrDir"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{FileSystemInfo}"/> that contains all <see cref="FileInfo"/> and <see cref="DirectoryInfo"/> from <see cref="CurrDir"/>.</returns>
        private IEnumerable<FileSystemInfo> CurrDirChildren() {
            try {
                var directories = CurrDir.GetDirectories().Where(x => !x.Attributes.HasFlag(FileAttributes.Hidden) && HasPermissions(x));
                var files = CurrDir.GetFiles().Where(x => !x.Attributes.HasFlag(FileAttributes.Hidden) && HasPermissions(x));

                return Enumerable.Concat<FileSystemInfo>(directories, files);
            } catch(Exception ex) {
                DarkMessageBox.Show("Error:", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Loads a new folder and displays its directories in <see cref="FoldersDataGrid"/> and its files in <see cref="FilesDataGrid"/>.
        /// </summary>
        /// <param name="folder">The folder to load.</param>
        /// <param name="search">A filter to show only related results. Leave empty to keep all results.</param>
        private void LoadFolder(DirectoryInfo folder, string search = "") {
            if(!folder.Exists) {
                DarkMessageBox.Show("Invalid path.", $"{folder.FullName} doesn't exist.");
            }

            PathDisplayTextBox.Text = folder.FullName;
            SelectedFolderNameTextBox.Text = string.Empty;


            var children = CurrDirChildren();
            if(children == null) {
                return;
            }

            FoldersDataGrid.ItemsSource = null;
            FilesDataGrid.ItemsSource = null;
            if(string.IsNullOrEmpty(search)) {
                FoldersDataGrid.ItemsSource = children.Where(x => x is DirectoryInfo);
                FilesDataGrid.ItemsSource = children.Where(x => x is FileInfo);
            } else {
                FoldersDataGrid.ItemsSource = children.Where(x => x.Name.Contains(search) && x is DirectoryInfo);
                FilesDataGrid.ItemsSource = children.Where(x => x.Name.Contains(search) && x is FileInfo);
            }

            SetIfActive();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            var parent = CurrDir.Parent;
            if(parent == null) {
                return;
            } else {
                LoadFolder(parent);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            LoadFolder(CurrDir, SearchTextBox.Text);
        }

        private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var folder = FileOrDirectory((sender as DataGridRow).Item);
            SelectedFolderNameTextBox.Text = folder.Name;
            LoadFolder(FullDir);
        }

        private void DataGridRow_Selected(object sender, RoutedEventArgs e) {
            var folder = FileOrDirectory((sender as DataGridRow).Item);
            SelectedFolderNameTextBox.Text = folder.Name;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
            this.Close();
        }

        /// <param name="path">The <see cref="FileSystemInfo"/> to check.</param>
        /// <returns>If the application has the permissions to check the AccessControl of a <see cref="FileSystemInfo"/></returns>
        private static bool HasPermissions(FileSystemInfo path) {
            try {

                if(path is DirectoryInfo) {
                    (path as DirectoryInfo).GetAccessControl();
                } else {
                    (path as FileInfo).GetAccessControl();
                }

                return true;
            } catch(UnauthorizedAccessException) {
                return false;
            }
        }

        /// <summary>
        /// Deactivates <see cref="BackButton"/> if there isn't a parent folder it can return to.
        /// </summary>
        private void SetIfActive() {
            BackButton.IsEnabled = CurrDir.Parent != null;
        }

        /// <summary>
        /// Takes <see cref="DataGridRow.Item"/> and converts it to either <see cref="DirectoryInfo"/> or <see cref="FileInfo"/>. If the conversion fails, it throws an <see cref="InvalidDataException"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>A <see cref="DirectoryInfo"/> if the object represents a directory or a <see cref="FileInfo"/> if it represents a file.</returns>
        private static FileSystemInfo FileOrDirectory(object item) {
            var dir = item as DirectoryInfo;
            if(dir is null) {
                return item as FileInfo;
            } else if(dir is not null) {
                return dir;
            } else {
                throw new InvalidDataException();
            }
        }

    }
}
