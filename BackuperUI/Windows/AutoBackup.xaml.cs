using BackuperLibrary;
using BackuperLibrary.UISpeaker;
using BackuperUI.UIClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BackuperUI.Windows {
    /// <summary>
    /// Interaction logic for AutoBackup.xaml
    /// </summary>
    public partial class AutoBackup : Window {

        private event EventHandler<IEnumerable<BackuperResultInfo>> workDone;
        private event EventHandler<Exception> workFailed;

        public AutoBackup() {
            InitializeComponent();
            workDone += AutoBackup_workDone;
            workFailed += AutoBackup_workFailed;

            //since this runs when the windows' user logins, this delay is to refrain from piling up
            //the backuping functionality on top of the currently initializing programs
            Thread.Sleep(10000);

            new Thread(async() => {
                Thread.CurrentThread.IsBackground = false;
                IEnumerable<BackuperResultInfo> results;

                try {
                    results = await BackupersHolder.BackupOnlyAsync(BackupersHolder.Backupers.Where(x => x.UpdateAutomatically));
                    workDone?.Invoke(this, results);
                } catch(Exception ex) {
                    workFailed?.Invoke(this, ex);
                }

            }).Start();
        }

        private void AutoBackup_workFailed(object sender, Exception e) {
            Dispatcher.Invoke(() => {
                Backuping.ShowError(e);
                Close();
            });
        }

        private void AutoBackup_workDone(object sender, IEnumerable<BackuperResultInfo> e) {
            Dispatcher.Invoke(() => {
                if(e is not null && e.Any()) {
                    Backuping.ShowResultsToUser(e);
                }

                Close();
            });

        }

    }
}
