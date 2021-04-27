using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BackuperLibrary;

namespace BackuperUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window { //todo - refactor all this class

        public MainWindow() {
            InitializeComponent();
            RefreshListBox();
        }

        private void RefreshListBox() {
            DataGridBackups.ItemsSource = null;
            var infoBackups = GlobalValues.Backupers.Select(x => new InfoBackup(x));
            DataGridBackups.ItemsSource = infoBackups;
        }
        
        private void StartBackupButton_Click(object sender, RoutedEventArgs e) {
            try {
                InfoBackup backup = (sender as Button).DataContext as InfoBackup;
                BackuperResultInfo status = GlobalValues.Backupers.Where(x => x.Name == backup.BackupName && x.From == backup.SourcePath).Single().MakeBackup();

                MessageBox.Show(status.GetMessage());
                RefreshListBox();
            } catch(Exception ex) {
                MessageBox.Show($"There has been an error: {0}", ex.Message);
            }
        }

        private void CreateBackuperButton_Click(object sender, RoutedEventArgs e) {
            BackuperCreator adder = new BackuperCreator();
            adder.Show();
        }

        private void DeleteBackuperButton_Click(object sender, RoutedEventArgs e) {
            var userAnswer = MessageBox.Show("Are you sure?", "Do you want to delete this automatic backup?", MessageBoxButton.YesNo);
            if(userAnswer != MessageBoxResult.Yes) {
                return;
            }

            try {
                InfoBackup backup = (sender as Button).DataContext as InfoBackup;

                userAnswer = MessageBox.Show($"Do you want to delete all the backups of {backup.BackupName}?", "Are you sure?", MessageBoxButton.YesNoCancel);

                if(userAnswer == MessageBoxResult.Cancel) {
                    MessageBox.Show("The deletion has been cancelled.");
                    return;
                }
                else if(userAnswer == MessageBoxResult.No) {
                    GlobalValues.Backupers.Remove(GlobalValues.Backupers.Where(x => x.Name == backup.BackupName && x.From == backup.SourcePath).Single());
                    MessageBox.Show("The element has been deleted, but the already-made backups have been left in the backup folder.");
                }
                else if(userAnswer == MessageBoxResult.Yes) {
                    var backuper = GlobalValues.Backupers.Where(x => x.Name == backup.BackupName && x.From == backup.SourcePath).Single();
                    string message = backuper.EraseBackups();
                    GlobalValues.Backupers.Remove(backuper);

                    MessageBox.Show(message);
                }
            } catch(Exception ex) {
                MessageBox.Show($"There was an error: {Environment.NewLine}{ex.Message}");
            } finally {
                RefreshListBox();
            }
        }

        private void BackupAllButton_Click(object sender, RoutedEventArgs e) {
            List<BackuperResultInfo> results = new List<BackuperResultInfo>();
            foreach(Backuper backuper in GlobalValues.Backupers) {
                BackuperResultInfo result = backuper.MakeBackup();
                results.Add(result);
            }

            int updated = results.Where(x => x.Result == BackuperResult.AlreadyUpdated).Count();
            int succeeded = results.Where(x => x.Result == BackuperResult.Success).Count();
            int errors = results.Where(x => x.Result == BackuperResult.Failure).Count();

            if(errors == 0) {
                MessageBox.Show(
                    $"{succeeded} have been backuped successfully.\r\n" +
                    $"{updated} were already updated.\r\n" +
                    $"{errors} met failure."
                    );
            } else {
                var userAnswer = MessageBox.Show(
                    $"{succeeded} have been backuped successfully.\r\n" +
                    $"{updated} were already updated.\r\n" +
                    $"{errors} met failure.\r\n \r\n" +
                    "Do you want to see the error messages?"
                    , "Backup Complete!", MessageBoxButton.YesNo);

                var failures = results.Where(x => x.Result == BackuperResult.Failure);
                foreach(BackuperResultInfo failure in failures) {
                    var answer = MessageBox.Show($"{failure.GetMessage()}{Environment.NewLine}{Environment.NewLine}If you don't want do read the other errors, choose \"NO\"", "Error:", MessageBoxButton.YesNo);
                    if(answer == MessageBoxResult.No) {
                        break;
                    }
                }
            }

            RefreshListBox();
        }

        private void ModifyBackuperButton_Click(object sender, RoutedEventArgs e) {

        }


        #region WindowBasicFunctionality
        private void MinimizeButton_Click(object sender, RoutedEventArgs e) {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e) {

            if(this.WindowState == WindowState.Normal) {

                this.WindowState = WindowState.Maximized;

            } else if(this.WindowState == WindowState.Maximized) {

                this.WindowState = WindowState.Normal;
            }
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
        #endregion
    }

    public class InfoBackup {
        public InfoBackup(Backuper backuper) {
            this.BackupName = backuper.Name;
            this.SourcePath = backuper.From;
            this.IsUpdated = backuper.IsUpdated;
        }
        
        public bool IsUpdated { get; set; } 
        public string BackupName { get; set; }
        public string SourcePath { get; set; }

    }
}
