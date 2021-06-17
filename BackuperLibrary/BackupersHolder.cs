using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackuperLibrary.IO;
using BackuperLibrary.UISpeaker;

namespace BackuperLibrary {
    public static class BackupersHolder {

        public static event EventHandler EditedBackupers;
        public static List<Backuper> Backupers { get; private set; } = BackupersManager.LoadAll();

        static BackupersHolder() {
            BackupersManager.EditedBackupers += Refresh;
            Backupers.ForEach(x => x.BackupComplete += Refresh);
        }

        private static void Refresh(object sender, EventArgs e) {
            Backupers = BackupersManager.LoadAll();
            EditedBackupers?.Invoke(null, EventArgs.Empty);
        }

        public static Backuper SearchByName(string name) {
            return Backupers.Where(x => x.Name == name).Single();
        }

        public static async Task<IEnumerable<BackuperResultInfo>> BackupAllAsync() {
            List<Task<BackuperResultInfo>> tasks = new List<Task<BackuperResultInfo>>();

            foreach(Backuper backuper in Backupers) {
                tasks.Add(
                    new Task<BackuperResultInfo>(() => {
                        if (Monitor.TryEnter(backuper)) {
                            try {
                                Thread.CurrentThread.IsBackground = false;
                                return backuper.MakeBackup();
                            } finally {
                                Monitor.Exit(backuper);
                                Thread.CurrentThread.IsBackground = true;
                            }
                        } else {
                            return new BackuperResultInfo(backuper.Name, BackuperResult.Failure, new ArgumentException("This backuper is being used elsewhere."));
                        }
                    })
                );
            }

            tasks.ForEach(x => x.Start());

            await Task.WhenAll(tasks.ToArray());

            return tasks.Select(x => x.Result);
        }

    }
}
