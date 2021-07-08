using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackuperLibrary.Generic;
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
            return await BackupOnlyAsync(Backupers);
        }

        public static async Task<IEnumerable<BackuperResultInfo>> BackupOnlyAsync(IEnumerable<Backuper> backupers) {
            List<Task<BackuperResultInfo>> tasks = new List<Task<BackuperResultInfo>>();

            foreach(Backuper backuper in backupers) {
                tasks.Add(
                    new Task<BackuperResultInfo>(() => {
                        if(backuper.CheckLock()) {

                            return Settings.SetThreadForegroundHere(() => {

                                try {
                                    return backuper.MakeBackup();
                                } catch(Exception ex) {
                                    return Factory.CreateBackupResult(backuper.Name, BackuperResult.Failure, ex);
                                }
                            });

                        } else {
                            return new BackuperResultInfo(backuper.Name, BackuperResult.Failure, new ArgumentException("This backuper is being used elsewhere."));
                        }
                    })
                );
            }

            tasks.ForEach(x => x.Start());

            await Task.WhenAll(tasks.ToArray());

            EditedBackupers?.Invoke(null, EventArgs.Empty);
            return tasks.Select(x => x.Result);
        }

        public static IEnumerable<BackuperResultInfo> BackupAll() {
            return BackupOnly(Backupers);
        }

        public static IEnumerable<BackuperResultInfo> BackupOnly(IEnumerable<Backuper> backupers) {
            return backupers.Select(x => x.MakeBackup());
        }

    }
}
