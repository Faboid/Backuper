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

namespace BackuperUI.Windows {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window { //todo - refactor all this class

        private readonly string messageErrorCaption = "There has been an error!";
        private readonly string operationFailedCaption = "Operation Failed.";
        private readonly string backuperIsUsedElsewhere = "This backuper is being used elsewhere.";

        public MainWindow() {
            BackuperLibrary.Generic.Settings.IFDEBUGSetCurrentThreadToEnglish();

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
        
        private async void StartBackupButton_Click(object sender, RoutedEventArgs e) {
            try {
                var btn = sender as Button;
                var backuper = btn.DataContext as Backuper;

                //To avoid blocking the UI thread
                await Task.Run(() => {
                    try {
                        Thread.CurrentThread.IsBackground = false;
                        BackuperResultInfo status = backuper.MakeBackup();
                        Dispatcher.Invoke(() => DarkMessageBox.Show("Result", status.GetMessage()));
                    } catch (ArgumentException ex) {
                        DarkMessageBox.Show(operationFailedCaption, ex.Message);
                    } catch(TaskCanceledException) { 
                        //do nothing
                    } catch(Exception ex) {
                        DarkMessageBox.Show(messageErrorCaption, ex.Message);
                    } finally {
                        Thread.CurrentThread.IsBackground = true;
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

                var backuper = (sender as Button).DataContext as Backuper;

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

                        } catch (ArgumentException ex) {
                            Dispatcher.Invoke(() => DarkMessageBox.Show(operationFailedCaption, ex.Message));
                        }
                    });

                }

            } catch(Exception ex) {
                DarkMessageBox.Show(messageErrorCaption, ex.Message);
            }
        }

        private async void BackupAllButton_Click(object sender, RoutedEventArgs e) {
            BackupAll_Button.IsEnabled = false;

            await Backuping.BackupAllAsync();

            BackupAll_Button.IsEnabled = true;
        }

        private void ModifyBackuperButton_Click(object sender, RoutedEventArgs e) {
            try {
                var backuper = (sender as Button).DataContext as Backuper;

                if(backuper.CheckLock()) {
                    BackuperEditor.Edit(backuper);
                } else {
                    DarkMessageBox.Show(operationFailedCaption, backuperIsUsedElsewhere);
                }

            } catch (ArgumentException ex) {
                DarkMessageBox.Show(operationFailedCaption, ex.Message);
            } catch(Exception ex) {
                DarkMessageBox.Show(messageErrorCaption, ex.Message);
            }
        }

        private void ChangeBackupPath_Button_Click(object sender, RoutedEventArgs e) {
            if(BackupersHolder.Backupers.All(x => x.CheckLock())) {
                try {
                    EditorBackupPath.Start();
                } catch (Exception ex) {
                    DarkMessageBox.Show(messageErrorCaption, ex.Message);
                }
            } else {
                DarkMessageBox.Show(operationFailedCaption, "Some backuper is being used elsewhere.");
            }
        }

        private void ToggleAutomaticBackupsButton_Click(object sender, RoutedEventArgs e) {
            ToggleAutomaticBackups_Button.IsEnabled = false;

            //swaps: if active turns inactive, if inactive turns active
            Startup.Set(!Startup.IsActive());

            RefreshToggleAutomaticBackupsButtonText();

            ToggleAutomaticBackups_Button.IsEnabled = true;
        }

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
