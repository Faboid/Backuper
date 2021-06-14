using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BackuperLibrary;
using BackuperLibrary.UISpeaker;
using BackuperLibrary.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace BackuperUI.Windows {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window { //todo - refactor all this class

        private readonly string messageErrorCaption = "There has been an error!";

        public MainWindow() {
            Settings.IFDEBUGSetCurrentThreadToEnglish();

            InitializeComponent();
            RefreshListBox(null, EventArgs.Empty);
            BackupersHolder.EditedBackupers += RefreshListBox;
        }

        private void RefreshListBox(object sender, EventArgs e) {
            Dispatcher.Invoke(() => {
                DataGridBackups.ItemsSource = null;;
                DataGridBackups.ItemsSource = BackupersHolder.Backupers.Select(x => new InfoBackuper(x));
            });
        }
        
        private async void StartBackupButton_Click(object sender, RoutedEventArgs e) {
            try {
                var btn = sender as Button;
                var backup = btn.DataContext as InfoBackuper;
                Backuper backuper = BackupersHolder.SearchByName(backup.BackupName);

                //To avoid blocking the UI thread
                await Task.Run(() => {
                    if(Monitor.TryEnter(backuper)) {
                        try {
                            BackuperResultInfo status = backuper.MakeBackup();
                            Dispatcher.Invoke(() => DarkMessageBox.Show("Result", status.GetMessage()));
                        } finally {
                            Monitor.Exit(backuper);
                        }
                    } else {
                        Dispatcher.Invoke(() => DarkMessageBox.Show("Error:", "This backuper is already being updated."));
                    }
                });


            } catch(Exception ex) {
                DarkMessageBox.Show(messageErrorCaption, ex.Message);
            }
        }

        private void CreateBackuperButton_Click(object sender, RoutedEventArgs e) {
            BackuperEditor.Create();
        }

        private async void DeleteBackuperButton_Click(object sender, RoutedEventArgs e) {
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
                    if (Monitor.TryEnter(backuper)) {
                        await Task.Run(() => {
                            string message = backuper.Erase(userAnswer == MessageBoxResult.Yes);
                            Dispatcher.Invoke(() => DarkMessageBox.Show("Operation Completed.", message));
                        });
                    } else {
                        DarkMessageBox.Show("Operation Failed.", "This backuper is being used elsewhere.");
                    }

                }

            } catch(Exception ex) {
                DarkMessageBox.Show(messageErrorCaption, ex.Message);
            }
        }

        private async void BackupAllButton_Click(object sender, RoutedEventArgs e) {
            BackupAll_Button.IsEnabled = false;
            IEnumerable<BackuperResultInfo> results = null;

            results = await BackupersHolder.BackupAllAsync();

            if(results is null) {
                DarkMessageBox.Show("Something went wrong!", "The list of the results is null.");
            }

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

                int totalcount = failures.Count();
                int currentcount = 0;
                string chooseMessage = $"{Environment.NewLine}{Environment.NewLine}Do you want to read the next error?";
                foreach(BackuperResultInfo failure in failures) {
                    currentcount++;

                    var answer = DarkMessageBox.Show("Error:", $"{failure.GetMessage()}{((currentcount == totalcount)? "" : chooseMessage)}", MessageBoxButton.YesNo);
                    if(answer == MessageBoxResult.No) {
                        break;
                    }
                }
            }

            BackupAll_Button.IsEnabled = true;
        }

        private void ModifyBackuperButton_Click(object sender, RoutedEventArgs e) {
            try {
                InfoBackuper backup = (sender as Button).DataContext as InfoBackuper;
                Backuper backuper = BackupersHolder.SearchByName(backup.BackupName);
                if(Monitor.TryEnter(backuper)) {
                    BackuperEditor.Edit(backuper);
                } else {
                    DarkMessageBox.Show("Error:", "This backuper is being used elsewhere.");
                }
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
