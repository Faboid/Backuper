using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BackuperLibrary.ErrorHandling;
using BackuperLibrary.IO;

namespace BackuperUI.Windows {

    /// <summary>
    /// A window to edit the backups' main path.
    /// </summary>
    public partial class EditorBackupPath : Window {

        /// <summary>
        /// Stars a new <see cref="EditorBackupPath"/> window and wait until it's closed.
        /// </summary>
        public static void Start() {
            var editor = new EditorBackupPath();
            editor.ShowDialog();
        }

        private EditorBackupPath() {
            InitializeComponent();
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e) {
            SetDefaultButton.IsEnabled = false;
            ConfirmButton.IsEnabled = false;

            string path = PathTextBox.Text;

            try {
                var dir = new DirectoryInfo(path);

                if(dir.Exists) {
                    bool success = false;

                    await Task.Run(() => {
                        success = BackupFolderHandler.TryChangePath(dir);
                    });

                    if(success) {
                        DarkMessageBox.Show("Success", "The backup path has been changed successfully.", Dispatcher);
                        this.Close();
                    } else {
                        DarkMessageBox.Show("Failure", "The operation has failed for unknown reasons.", Dispatcher);
                    }
                } else {

                    DarkMessageBox.Show("Error:", "Given path must exist.", Dispatcher);
                }

            } catch(IOException ex) {

                DarkMessageBox.Show("There has been an error", ex.Message, Dispatcher);
            } finally {
                SetDefaultButton.IsEnabled = true;
                ConfirmButton.IsEnabled = true;
            }

        }

        private void SetDefaultButton_Click(object sender, RoutedEventArgs e) {
            SetDefaultButton.IsEnabled = false;
            ConfirmButton.IsEnabled = false;

            try {
                bool success = BackupFolderHandler.SetDefault();
                if(success) {
                    DarkMessageBox.Show("Success", "Set to default successfully.", Dispatcher);
                } else {
                    DarkMessageBox.Show("Failure", "Something went wrong.", Dispatcher);
                }
            } catch(IOException ex) {

                DarkMessageBox.Show("There has been an error:", ex.Message, Dispatcher);
            } finally {
                SetDefaultButton.IsEnabled = true;
                ConfirmButton.IsEnabled = true;
            }
        }

    }
}
