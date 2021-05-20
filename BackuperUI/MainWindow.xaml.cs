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
using BackuperLibrary.UISpeaker;

namespace BackuperUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window { //todo - refactor all this class

        private string messageErrorCaption = "There has been an error";

        public MainWindow() {
            InitializeComponent();
            RefreshListBox();
        }

        private void RefreshListBox() {
            DataGridBackups.ItemsSource = null;
            var infoBackups = BackupersHolder.Backupers.Select(x => new InfoBackuper(x));
            DataGridBackups.ItemsSource = infoBackups;
        }
        
        private void StartBackupButton_Click(object sender, RoutedEventArgs e) {
            try {
                InfoBackuper backup = (sender as Button).DataContext as InfoBackuper;
                BackuperResultInfo status = BackupersHolder.SearchByName(backup.BackupName).MakeBackup();

                MessageBox.Show(status.GetMessage(), "Result");
                RefreshListBox();
            } catch(Exception ex) {
                MessageBox.Show(ex.Message, messageErrorCaption);
            }
        }

        private void CreateBackuperButton_Click(object sender, RoutedEventArgs e) {
            BackuperEditor.Create();
            RefreshListBox();
        }

        private void DeleteBackuperButton_Click(object sender, RoutedEventArgs e) {
            var userAnswer = MessageBox.Show("Do you want to delete this backuper?", "Are you sure?", MessageBoxButton.YesNo);
            if(userAnswer != MessageBoxResult.Yes) {
                return;
            }

            try {
                InfoBackuper backup = (sender as Button).DataContext as InfoBackuper;

                userAnswer = MessageBox.Show($"Do you want to delete all the backups of {backup.BackupName}? {Environment.NewLine}" +
                    $"Replying \"No\" will delete the backuper, but won't delete the files.", "Are you sure?", MessageBoxButton.YesNoCancel);

                if(userAnswer == MessageBoxResult.Cancel) {
                    MessageBox.Show("The operation has been annulled.");
                    return;

                } else if(userAnswer == MessageBoxResult.No || userAnswer == MessageBoxResult.Yes) {
                    Backuper backuper = BackupersHolder.SearchByName(backup.BackupName);
                    string message = backuper.Erase(userAnswer == MessageBoxResult.Yes);
                    MessageBox.Show(message);
                }

            } catch(Exception ex) {
                MessageBox.Show(ex.Message, messageErrorCaption);
            } finally {
                RefreshListBox();
            }
        }

        private void BackupAllButton_Click(object sender, RoutedEventArgs e) {
            List<BackuperResultInfo> results = BackupersHolder.BackupAll();
            ResultsHandler.GetResults(results, out int succeeded, out int updated, out int errors);

            RefreshListBox();


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

                if(userAnswer == MessageBoxResult.No) {
                    return;
                }

                var failures = results.Where(x => x.Result == BackuperResult.Failure);
                foreach(BackuperResultInfo failure in failures) {
                    var answer = MessageBox.Show($"{failure.GetMessage()}{Environment.NewLine}{Environment.NewLine}If you don't want do read the other errors, choose \"NO\"", "Error:", MessageBoxButton.YesNo);
                    if(answer == MessageBoxResult.No) {
                        break;
                    }
                }
            }
        }

        private void ModifyBackuperButton_Click(object sender, RoutedEventArgs e) {
            try {
                InfoBackuper backup = (sender as Button).DataContext as InfoBackuper;
                Backuper backuper = BackupersHolder.SearchByName(backup.BackupName);
                BackuperEditor.Edit(backuper);
            } catch(Exception ex) {
                MessageBox.Show(ex.Message, messageErrorCaption);
            }
            RefreshListBox();
        }

        private void ChangeBackupPath_Button_Click(object sender, RoutedEventArgs e) {

        }
    }

    internal class InfoBackuper {
        internal InfoBackuper(Backuper backuper) {
            this.BackupName = backuper.Name;
            this.SourcePath = backuper.From;
            this.IsUpdated = backuper.IsUpdated;
        }
        
        public bool IsUpdated { get; set; } 
        public string BackupName { get; set; }
        public string SourcePath { get; set; }

    }
}
