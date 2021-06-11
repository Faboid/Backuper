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
using BackuperLibrary.IO;

namespace BackuperUI.Windows {
    /// <summary>
    /// Interaction logic for EditorBackupPath.xaml
    /// </summary>
    public partial class EditorBackupPath : Window {

        public static void Start() {
            var editor = new EditorBackupPath();
            editor.ShowDialog();
        }

        public EditorBackupPath() {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e) {
            string path = PathTextBox.Text;

            try {

                if(Directory.Exists(path)) {
                    bool success = BackupFolderHandler.TryChangePath(path);

                    if(success) {
                        DarkMessageBox.Show("Success", "The backup path has been changed successfully.");
                        this.Close();
                    } else {
                        DarkMessageBox.Show("Failure", "The operation has failed for unknown reasons.");
                    }
                } else {
                    DarkMessageBox.Show("Error", "Given path must exist.");
                }

            } catch(Exception ex) {
                DarkMessageBox.Show("There has been an error", ex.Message);
            }

        }

        private void SetDefaultButton_Click(object sender, RoutedEventArgs e) {
            try {
                bool success = BackupFolderHandler.SetDefault();
                if(success) {
                    DarkMessageBox.Show("Success", "Set to default successfully.");
                } else {
                    DarkMessageBox.Show("Failure", "Something went wrong.");
                }
            } catch (Exception ex) {
                DarkMessageBox.Show("There has been an error:", ex.Message);
            }
        }

    }
}
