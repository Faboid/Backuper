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
    /// Interaction logic for OpenFolderDialog.xaml
    /// </summary>
    public partial class OpenFolderDialog : Window {

        public static new string Show() {
            var dialog = new OpenFolderDialog();
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


        public OpenFolderDialog() {
            InitializeComponent();
            LoadFolder(DefaultPath);
            var drives = DriveInfo.GetDrives();
            var roots = drives.Where(x => x.IsReady).Select(x => x.RootDirectory);
            RootsDataGrid.ItemsSource = roots;
        }

        private IEnumerable<DirectoryInfo> CurrDirChildren() {
            try {
                return CurrDir.GetDirectories().Where(x => !x.Attributes.HasFlag(FileAttributes.Hidden) && HasPermissions(x));
            } catch(Exception ex) {
                DarkMessageBox.Show("Error:", ex.Message);
                return null;
            }
        }

        private void LoadFolder(DirectoryInfo folder, string search = "") {
            PathDisplayTextBox.Text = folder.FullName;
            SelectedFolderNameTextBox.Text = string.Empty;


            var children = CurrDirChildren();
            if(children == null) {
                return;
            }

            FoldersDataGrid.ItemsSource = null;
            if(search == "") {
                FoldersDataGrid.ItemsSource = children;
            } else {
                FoldersDataGrid.ItemsSource = children.Where(x => x.Name.Contains(search));
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
            var folder = (sender as DataGridRow).Item as DirectoryInfo;
            SelectedFolderNameTextBox.Text = folder.Name;
            LoadFolder(FullDir);
        }

        private void DataGridRow_Selected(object sender, RoutedEventArgs e) {
            var folder = (sender as DataGridRow).Item as DirectoryInfo;
            SelectedFolderNameTextBox.Text = folder.Name;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
            this.Close();
        }
        private static bool HasPermissions(DirectoryInfo dir) {
            try {
                dir.GetAccessControl();
                return true;
            } catch(UnauthorizedAccessException) {
                return false;
            }
        }

        private void SetIfActive() {
            BackButton.IsEnabled = CurrDir.Parent != null;
        }

    }
}
