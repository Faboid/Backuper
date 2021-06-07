using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Security.AccessControl;

using BackuperLibrary.Generic;

namespace BackuperUI.Windows {
    /// <summary>
    /// Interaction logic for OpenFolderDialog.xaml
    /// </summary>
    public partial class OpenFolderDialog : Window {

        public new static string Show() {
            Settings.SetCurrentThreadToEnglish();

            var dialog = new OpenFolderDialog();
            if(dialog.ShowDialog() == true) {
                return dialog.fullPath;
            } else {
                return null;
            }
        }


        private DirectoryInfo currDir { get => new DirectoryInfo(PathDisplayTextBox.Text); }
        private DirectoryInfo fullDir { get => new DirectoryInfo(fullPath); }
        private string fullPath { get => Path.Combine(PathDisplayTextBox.Text, SelectedFolderNameTextBox.Text); }
        private DirectoryInfo defaultPath { get; } = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));


        public OpenFolderDialog() {
            InitializeComponent();
            LoadFolder(defaultPath);
            var drives = DriveInfo.GetDrives();
            var roots = drives.Where(x => x.IsReady).Select(x => x.RootDirectory);
            RootsDataGrid.ItemsSource = roots;
        }

        private bool HasPermissions(DirectoryInfo dir) {
            try {
                dir.GetAccessControl();
                return true;
            } catch(UnauthorizedAccessException) {
                return false;
            }
        }

        private IEnumerable<DirectoryInfo> currDirChildren() {
            try {
                return currDir.GetDirectories().Where(x => !x.Attributes.HasFlag(FileAttributes.Hidden) && HasPermissions(x));
            } catch(Exception ex) {
                DarkMessageBox.Show("Error:", ex.Message);
                return null;
            }
        }

        private void LoadFolder(DirectoryInfo folder, string search = "") {
            PathDisplayTextBox.Text = folder.FullName;
            SelectedFolderNameTextBox.Text = string.Empty;


            var children = currDirChildren();
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
            var parent = currDir.Parent;
            if(parent == null) {
                return;
            } else {
                LoadFolder(parent);
            }
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            LoadFolder(currDir, SearchTextBox.Text);
        }

        private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var folder = (sender as System.Windows.Controls.DataGridRow).Item as DirectoryInfo;
            SelectedFolderNameTextBox.Text = folder.Name;
            LoadFolder(fullDir);
        }

        private void DataGridRow_Selected(object sender, RoutedEventArgs e) {
            var folder = (sender as System.Windows.Controls.DataGridRow).Item as DirectoryInfo;
            SelectedFolderNameTextBox.Text = folder.Name;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
            this.Close();
        }

        private void SetIfActive() {
            BackButton.IsEnabled = currDir.Parent != null;
        }

    }
}
