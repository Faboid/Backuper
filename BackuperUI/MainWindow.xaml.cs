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
    public partial class MainWindow : Window {

        List<Backuper> backupers = GetSampleBackupers(); //todo - automatically get saved backups paths

        //temporary function until more basic functionalities get added
        private static List<Backuper> GetSampleBackupers() {
            var backupers = new List<Backuper>();
            backupers.Add(new Backuper(@"D:\Programming\Small Projects\Backuper\TemporaryTestFolder\From", "Test", 5));
            backupers.Add(new Backuper(@"D:\Programming\Small Projects\Backuper\TemporaryTestFolder\From", "SecondTest", 5));
            backupers.Add(new Backuper(@"D:\Programming\Small Projects\Backuper\TemporaryTestFolder\From", "ThirdTest", 5));
            return backupers;
        }

        public MainWindow() {
            InitializeComponent();
            RefreshListBox();
        }

        private void RefreshListBox() {
            DataGridBackups.ItemsSource = null;
            var infoBackups = backupers.Select(x => new InfoBackup(x));
            DataGridBackups.ItemsSource = infoBackups;
        }
        
        private void StartBackupButton_Click(object sender, RoutedEventArgs e) {
            try {
                InfoBackup backup = (sender as Button).DataContext as InfoBackup;
                string message = backupers.Where(x => x.Name == backup.BackupName && x.From == backup.SourcePath).Single().MakeBackup();

                MessageBox.Show(message);
                RefreshListBox();
            } catch(Exception ex) {
                MessageBox.Show($"There has been an error: {0}", ex.Message);
            }
        }

        private void DeleteBackupButton_Click(object sender, RoutedEventArgs e) {
            try {
                InfoBackup backup = (sender as Button).DataContext as InfoBackup;

                var userAnswer = MessageBox.Show($"Do you want to delete all the backups of {backup.BackupName}?", "Are you sure?", MessageBoxButton.YesNoCancel);

                if(userAnswer == MessageBoxResult.Cancel) {
                    MessageBox.Show("The deletion has been cancelled.");
                    return;
                }
                else if(userAnswer == MessageBoxResult.No) {
                    backupers.Remove(backupers.Where(x => x.Name == backup.BackupName && x.From == backup.SourcePath).Single());
                    MessageBox.Show("The element has been deleted, but the already-made backups have been left in the backup folder.");
                }
                else if(userAnswer == MessageBoxResult.Yes) {
                    var backuper = backupers.Where(x => x.Name == backup.BackupName && x.From == backup.SourcePath).Single();
                    string message = backuper.EraseBackups();
                    backupers.Remove(backuper);

                    MessageBox.Show(message);
                }
            } catch(Exception ex) {
                MessageBox.Show($"There was an error: {Environment.NewLine}{ex.Message}");
            } finally {
                RefreshListBox();
            }
        }

        private void BackupAllButton_Click(object sender, RoutedEventArgs e) {
            List<string> messages = new List<string>();
            foreach(Backuper backuper in backupers) {
                var message = backuper.MakeBackup();
                messages.Add(message);
            }
            int updated = 0;
            int succeeded = 0;
            int errors = 0;

            //todo - finish this
            foreach(string message in messages) {
                if(message == Backuper.ALREADY_UPDATED_MESSAGE) {
                    updated++;
                } 
                else if(message == Backuper.SUCCESSFUL_BACKUP_MESSAGE) {
                    succeeded++;
                } 
                else {
                    errors++;
                }
            }
        }
    }

    public class InfoBackup {
        public InfoBackup(Backuper backuper) {
            this.BackupName = backuper.Name;
            this.SourcePath = backuper.From;
            this.IsUpdated = backuper.IsUpdated;
        }
        
        public bool IsUpdated { get; set; } //todo - implement getting one of the two(three?) images based on the result.
        public string BackupName { get; set; }
        public string SourcePath { get; set; }

    }
}
