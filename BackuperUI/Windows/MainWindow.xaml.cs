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
using BackuperUI.UIClasses;
using BackuperLibrary.ErrorHandling;

namespace BackuperUI.Windows {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window { //todo - refactor all this class

        private readonly string messageErrorCaption = "There has been an error!";
        private readonly string operationFailedCaption = "Operation Failed.";
        private readonly string backuperIsUsedElsewhereCaption = "This backuper is being used elsewhere.";

        public MainWindow() {
            Settings.IFDEBUGSetCurrentThreadToEnglish();

            InitializeComponent();
            RefreshListBox(null, EventArgs.Empty);
            BackupersHolder.EditedBackupers += RefreshListBox;
            RefreshToggleAutomaticBackupsButtonText();
        }

        private void RefreshListBox(object sender, EventArgs e) {
            Dispatcher.Invoke(() => {
                DataGridBackups.ItemsSource = null; ;
                DataGridBackups.ItemsSource = BackupersHolder.Backupers;
            });
        }
        
        /// <summary>
        /// If the backuper is free, starts its backup.
        /// </summary>
        private async void StartBackupButton_Click(object sender, RoutedEventArgs e) {
            try {
                var btn = sender as Button;
                var backuper = btn.DataContext as Backuper;

                //To avoid blocking the UI thread
                await Task.Run(() => {
                    try {
                        if(!backuper.CheckLock()) {
                            Dispatcher.Invoke(() => DarkMessageBox.Show(operationFailedCaption, backuperIsUsedElsewhereCaption));
                            return;
                        }

                        Thread.CurrentThread.IsBackground = false;
                        BackuperResultInfo status = backuper.MakeBackup();
                        Dispatcher.Invoke(() => DarkMessageBox.Show("Result", status.GetMessage()));
                    } catch(TaskCanceledException) { 
                        //do nothing
                    } catch(Exception ex) {
                        Log.WriteError(ex);
                        Dispatcher.Invoke(() => DarkMessageBox.Show(messageErrorCaption, ex.Message));
                        
                    } finally {
                        Thread.CurrentThread.IsBackground = true;
                    }
                });


            } catch(Exception ex) {
                Log.WriteError(ex);
                DarkMessageBox.Show(messageErrorCaption, ex.Message);
            }
        }

        /// <summary>
        /// Starts a window of <see cref="BackuperEditor"/> to create a new backuper.
        /// </summary>
        private void CreateBackuperButton_Click(object sender, RoutedEventArgs e) {
            BackuperEditor.Create();
        }

        /// <summary>
        /// Asks for confirmation for the user, then, acts accordingly, 
        /// either cancelling the operation, deleting the backuper and backups, 
        /// or deleting the backuper and moving the backups to a bin folder.
        /// </summary>
        private async void DeleteBackuperButton_Click(object sender, RoutedEventArgs e) {
            var userAnswer = DarkMessageBox.Show("Are you sure?", "Do you want to delete this backuper?", MessageBoxButton.YesNo);
            if(userAnswer != MessageBoxResult.Yes) {
                return;
            }

            Backuper backuper;
            try {
                backuper = (sender as Button).DataContext as Backuper;
            } catch (Exception ex) {
                Log.WriteError(ex);
                DarkMessageBox.Show(messageErrorCaption, ex.Message);
                return;
            }

            userAnswer = DarkMessageBox.Show("Are you sure?", $"Do you want to delete all the backups of {backuper.Name}? {Environment.NewLine}" +
                $"Replying \"No\" will delete the backuper, but won't delete the files.", MessageBoxButton.YesNoCancel);

            if(userAnswer == MessageBoxResult.Cancel) {
                DarkMessageBox.Show(string.Empty, "The operation has been annulled.");
                return;

            } else if(userAnswer == MessageBoxResult.No || userAnswer == MessageBoxResult.Yes) {

                await Task.Run(() => {
                    string message = "";
                    try {
                        message = backuper.Erase(userAnswer == MessageBoxResult.Yes);
                        Dispatcher.Invoke(() => DarkMessageBox.Show("Operation Completed.", message));

                    } catch (Exception ex) {
                        Log.WriteError(ex);
                        Dispatcher.Invoke(() => DarkMessageBox.Show(operationFailedCaption, ex.Message));
                    }
                });

            }
        }

        /// <summary>
        /// Tries to backup all buttons, then shows results to the user.
        /// </summary>
        private async void BackupAllButton_Click(object sender, RoutedEventArgs e) {
            BackupAll_Button.IsEnabled = false;

            try {
                var results = await BackupersHolder.BackupAllAsync();
                Backuping.ShowResultsToUser(results, Dispatcher);
            } catch(Exception ex) {

                Log.WriteError(ex);
                Backuping.ShowError(ex, Dispatcher);
            }

            BackupAll_Button.IsEnabled = true;
        }

        /// <summary>
        /// If the backuper is free, opens a new instance of <see cref="BackuperEditor"/> to edit it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyBackuperButton_Click(object sender, RoutedEventArgs e) {
            try {
                var backuper = (sender as Button).DataContext as Backuper;

                if(backuper.CheckLock()) {
                    BackuperEditor.Edit(backuper);
                } else {
                    DarkMessageBox.Show(operationFailedCaption, backuperIsUsedElsewhereCaption);
                }

            } catch (ArgumentException ex) {

                DarkMessageBox.Show(operationFailedCaption, ex.Message);
            } catch(Exception ex) {

                Log.WriteError(ex);
                DarkMessageBox.Show(messageErrorCaption, ex.Message);
            }
        }

        /// <summary>
        /// If all backupers are free, initializes an instance of <see cref="EditorBackupPath"/> to edit the current main backups path.
        /// </summary>
        private void ChangeBackupPath_Button_Click(object sender, RoutedEventArgs e) {
            if(BackupersHolder.Backupers.All(x => x.CheckLock())) {
                try {
                    EditorBackupPath.Start();
                } catch (Exception ex) {
                    Log.WriteError(ex);
                    DarkMessageBox.Show(messageErrorCaption, ex.Message);
                }
            } else {
                DarkMessageBox.Show(operationFailedCaption, "Some backuper is being used elsewhere.");
            }
        }

        /// <summary>
        /// Toggles automatic start up ON/OFF.
        /// </summary>
        private void ToggleAutomaticBackupsButton_Click(object sender, RoutedEventArgs e) {
            ToggleAutomaticBackups_Button.IsEnabled = false;

            //swaps: if active turns inactive, if inactive turns active
            Startup.Set(!Startup.IsActive());

            RefreshToggleAutomaticBackupsButtonText();

            ToggleAutomaticBackups_Button.IsEnabled = true;
        }

        /// <summary>
        /// Checks if the automatic startup is active, then edits <see cref="ToggleAutomaticBackups_Button"/>'s content accordingly.
        /// </summary>
        private void RefreshToggleAutomaticBackupsButtonText() {
            string preText = "Turn";
            string postText = "Automatic Backuping";

            if(Startup.IsActive()) {
                ToggleAutomaticBackups_Button.Content = $"{preText} OFF {postText}";
            } else {
                ToggleAutomaticBackups_Button.Content = $"{preText} ON {postText}";
            }
        }

    }

}
