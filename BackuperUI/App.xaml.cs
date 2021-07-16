using BackuperLibrary;
using BackuperLibrary.UISpeaker;
using BackuperUI.UIClasses;
using BackuperUI.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BackuperUI {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            MessageUI.Mail += ReceiveMessage;

            if(e.Args.Length is 0) {
                //run the app as normal
                MainWindow window = new MainWindow();
                window.Show();
            } else {
                if(e.Args[0] is BackuperLibrary.Startup.startupArgument) {
                    //open a window that backups all the backupers with UpdateAutomatically set to true
                    AutoBackup autobackup = new AutoBackup();
                    autobackup.Show();
                }
            }
        }

        private void ReceiveMessage(object sender, MailArgs e) {
            DarkMessageBox.Show(e.Title, e.Message, Dispatcher);
        }
    }
}
