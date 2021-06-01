using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BackuperLibrary;
using BackuperLibrary.UISpeaker;
using BackuperUI.Windows;
using BackuperLibrary.Generic;

namespace BackuperUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window { //todo - refactor all this class

        private readonly string messageErrorCaption = "There has been an error!";

        public MainWindow() {

#if DEBUG
            Settings.SetCurrentThreadToEnglish();
#endif

            InitializeComponent();
            RefreshListBox(null, EventArgs.Empty);
            BackupersHolder.EditedBackupers += RefreshListBox;
            BackupersHolder.Backupers.ForEach(x => x.BackupComplete += RefreshListBox);
        }

        private void RefreshListBox(object sender, EventArgs e) {
            DataGridBackups.ItemsSource = null;
            var infoBackups = BackupersHolder.Backupers.Select(x => new InfoBackuper(x));
            DataGridBackups.ItemsSource = infoBackups;
        }
        
        private void StartBackupButton_Click(object sender, RoutedEventArgs e) {
            try {
                InfoBackuper backup = (sender as Button).DataContext as InfoBackuper;
                BackuperResultInfo status = BackupersHolder.SearchByName(backup.BackupName).MakeBackup();

                DarkMessageBox.Show("Result", status.GetMessage());
            } catch(Exception ex) {
                DarkMessageBox.Show(messageErrorCaption, ex.Message);
            }
        }

        private void CreateBackuperButton_Click(object sender, RoutedEventArgs e) {
            BackuperEditor.Create();
        }

        private void DeleteBackuperButton_Click(object sender, RoutedEventArgs e) {
            var userAnswer = DarkMessageBox.Show("Are you sure?", "Do you want to delete this backuper?", MessageBoxButton.YesNo);
            if(userAnswer != MessageBoxResult.Yes) {
                return;
            }

            try {
                InfoBackuper backup = (sender as Button).DataContext as InfoBackuper;

                userAnswer = DarkMessageBox.Show("Are you sure?", $"Do you want to delete all the backups of {backup.BackupName}? {Environment.NewLine}" +
                    $"Replying \"No\" will delete the backuper, but won't delete the files.", MessageBoxButton.YesNoCancel);

                if(userAnswer == MessageBoxResult.Cancel) {
                    DarkMessageBox.Show(string.Empty, "The operation has been annulled.");
                    return;

                } else if(userAnswer == MessageBoxResult.No || userAnswer == MessageBoxResult.Yes) {
                    Backuper backuper = BackupersHolder.SearchByName(backup.BackupName);
                    string message = backuper.Erase(userAnswer == MessageBoxResult.Yes);
                    DarkMessageBox.Show("Operation completed.", message);
                }

            } catch(Exception ex) {
                DarkMessageBox.Show(messageErrorCaption, ex.Message);
            }
        }

        private void BackupAllButton_Click(object sender, RoutedEventArgs e) {
            List<BackuperResultInfo> results = BackupersHolder.BackupAll();
            ResultsHandler.GetResults(results, out int succeeded, out int updated, out int errors);


            if(errors == 0) {
                DarkMessageBox.Show("Backup Complete!",
                    $"{succeeded} have been backuped successfully.\r\n" +
                    $"{updated} were already updated.\r\n" +
                    $"{errors} met failure."
                    );
            } else {
                var userAnswer = DarkMessageBox.Show("Backup Complete!",
                    $"{succeeded} have been backuped successfully.\r\n" +
                    $"{updated} were already updated.\r\n" +
                    $"{errors} met failure.\r\n \r\n" +
                    "Do you want to see the error messages?"
                    , MessageBoxButton.YesNo);

                if(userAnswer == MessageBoxResult.No) {
                    return;
                }

                var failures = results.Where(x => x.Result == BackuperResult.Failure);
                foreach(BackuperResultInfo failure in failures) {
                    var answer = DarkMessageBox.Show("Error:", $"{failure.GetMessage()}{Environment.NewLine}{Environment.NewLine}If you don't want do read the other errors, choose \"NO\"", MessageBoxButton.YesNo);
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
                DarkMessageBox.Show(messageErrorCaption, ex.Message);
            }
        }

        private void ChangeBackupPath_Button_Click(object sender, RoutedEventArgs e) => EditorBackupPath.Start();
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
