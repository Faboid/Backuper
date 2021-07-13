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

    /// <summary>
    /// Keeps a static list of <see cref="Backuper"/> and gives way to interact with them.
    /// </summary>
    public static class BackupersHolder {

        /// <summary>
        /// Fires whenever a backupers-related operation is executed.
        /// </summary>
        public static event EventHandler EditedBackupers;

        /// <summary>
        /// A list of all <see cref="Backuper"/>.
        /// </summary>
        public static List<Backuper> Backupers { get; private set; } = BackupersManager.LoadAll();

        static BackupersHolder() {
            BackupersManager.EditedBackupers += Refresh;
            Backupers.ForEach(x => x.BackupComplete += Refresh);
        }

        private static void Refresh(object sender, EventArgs e) {
            Backupers = BackupersManager.LoadAll();
            EditedBackupers?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Searches a specific backuper by its unique name.
        /// </summary>
        /// <param name="name">The name to search.</param>
        /// <returns>The found backuper.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static Backuper SearchByName(string name) {
            return Backupers.Where(x => x.Name == name).Single();
        }

        /// <summary>
        /// Backups all <see cref="Backuper"/> in <see cref="Backupers"/> asynchronously.
        /// </summary>
        /// <returns>A list of <see cref="BackuperResultInfo"/> to relay the results of the backups.</returns>
        public static async Task<IEnumerable<BackuperResultInfo>> BackupAllAsync() {
            return await BackupOnlyAsync(Backupers);
        }

        /// <summary>
        /// Backups all <see cref="Backuper"/> in <paramref name="backupers"/> asynchronously.
        /// </summary>
        /// <param name="backupers">The list of backupers to backup.</param>
        /// <returns>A list of <see cref="BackuperResultInfo"/> to relay the results of the backups.</returns>
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

        /// <summary>
        /// Backups all <see cref="Backuper"/> in <see cref="Backupers"/> synchronously.
        /// </summary>
        /// <returns>A list of <see cref="BackuperResultInfo"/> to relay the results of the backups.</returns>
        public static IEnumerable<BackuperResultInfo> BackupAll() {
            return BackupOnly(Backupers);
        }

        /// <summary>
        /// Backups all <see cref="Backuper"/> in <paramref name="backupers"/> synchronously.
        /// </summary>
        /// <param name="backupers">The list of backupers to backup.</param>
        /// <returns>A list of <see cref="BackuperResultInfo"/> to relay the results of the backups.</returns>
        public static IEnumerable<BackuperResultInfo> BackupOnly(IEnumerable<Backuper> backupers) {
            return backupers.Select(x => x.MakeBackup());
        }

    }
}
