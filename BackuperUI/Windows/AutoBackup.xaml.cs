using BackuperLibrary;
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

        private event EventHandler workDone;


        public AutoBackup() {
            InitializeComponent();
            workDone += AutoBackup_workDone;

            //since this runs when the windows' user logins, this delay is to refrain from piling up
            //the backuping functionality on top of the currently initializing programs
            Thread.Sleep(10000);

            new Thread(() => {
                Thread.CurrentThread.IsBackground = false;
                Dispatcher.Invoke(() => Backuping.BackupOnlyAsync(BackupersHolder.Backupers.Where(x => x.UpdateAutomatically)));
                workDone?.Invoke(null, null);
            }).Start();
        }

        private void AutoBackup_workDone(object sender, EventArgs e) {
            Dispatcher.Invoke(() => Close());
        }

    }
}
