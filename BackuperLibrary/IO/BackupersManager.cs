using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using BackuperLibrary.Generic;
using BackuperLibrary.UISpeaker;

namespace BackuperLibrary.IO {
    public static class BackupersManager {

        /// <summary>
        /// Fires whenever any backuper is edited.
        /// </summary>
        public static event EventHandler EditedBackupers;

        /// <summary>
        /// Returns a path to the folder that contains the files of the backupers.
        /// </summary>
        public static string BackupersPath { get; } = @$"{PathBuilder.GetWorkingDirectory()}\Backupers";

        /// <summary>
        /// Returns a list with all the names of all backupers.
        /// </summary>
        public static List<string> BackupersNames { get; private set; } = GetBackupersNames();


        static BackupersManager() {
            EditedBackupers += RefreshBackupersNames;
        }

        /// <summary>
        /// Updates all the backupers' information to files.
        /// </summary>
        /// <param name="backupers">The list of backupers to save.</param>
        internal static void SaveAll(this List<Backuper> backupers) {
            backupers.ForEach(x => Save(x));
            EditedBackupers?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Saves a backuper's values to file.
        /// </summary>
        /// <param name="backuper">The backuper to save.</param>
        internal static void Save(this Backuper backuper) {
            string path = GetBackuperPath(backuper.Name);
            string content = backuper.ToString();
            string[] lines = content.Split(',');
            File.WriteAllLines(path, lines);
            EditedBackupers?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Renames a backuper's file.
        /// </summary>
        /// <param name="sender">The backuper to rename.</param>
        /// <param name="pastName">The previous name.</param>
        /// <param name="newName">The new name.</param>
        internal static void EditName(this Backuper sender, string pastName, string newName) {
            string pastPath = GetBackuperPath(pastName);
            string newPath = GetBackuperPath(newName);
            File.Copy(pastPath, newPath);
            File.Delete(pastPath);
            Save(sender);
            EditedBackupers?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Deletes a backuper's save file. Note: this method won't touch the backupers' already made backups.
        /// </summary>
        /// <param name="backuper">The backuper to delete.</param>
        internal static void Delete(this Backuper backuper) {
            string path = GetBackuperPath(backuper.Name);
            File.Delete(path);
            EditedBackupers?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Loads all backupers from their save file.
        /// </summary>
        /// <returns>A list of backupers.</returns>
        public static List<Backuper> LoadAll() {
            List<Backuper> backupers = new List<Backuper>();

            var directory = new DirectoryInfo(BackupersPath);

            if(!directory.Exists) {
                directory.Create();
                return backupers;
            }

            var files = directory.GetFiles();

            foreach(FileInfo file in files) {
                try {
                    var backuper = Load(file.FullName);
                    backupers.Add(backuper);
                } catch(Exception ex) {
                    ErrorHandling.Log.WriteError(ex);
                    bool result = DeleteCorruptedBackuper(file);

                    string name = Path.GetFileNameWithoutExtension(file.FullName);
                    if(result is true) {

                        MessageUI.Send(null, "Error on load!", $"There has been an error when trying to load the backuper {name}.{Environment.NewLine}{Environment.NewLine}" +
                                $"To protect against corrupted files, the backuper has been deleted, and its backups have been moved to the bin folder:" +
                                $"{Environment.NewLine}{PathBuilder.BinBackupsFolder}");
                    } else {

                        MessageUI.Send(null, "Error on load!", $"There has been an error when trying to load the backuper {name}.{Environment.NewLine}{Environment.NewLine}" +
                                $"To protect against corrupted files, the backuper has been deleted. {Environment.NewLine}" +
                                $"An attempt was made to move its backups to the bin folder, but an error occurred. It's suggested to manually check for the backups.");
                    }
                }
            }

            return backupers;
        }

        #region private

        /// <returns>A path to a specific backuper's save file.</returns>
        private static string GetBackuperPath(string name) => Path.Combine(BackupersPath, $"{name}.txt");

        private static void RefreshBackupersNames(object sender, EventArgs e) {
            BackupersNames = GetBackupersNames();
        }

        private static bool DeleteCorruptedBackuper(FileInfo backuperFile) {

            try {
                //gets name of the backuper
                string name = Path.GetFileNameWithoutExtension(backuperFile.FullName);

                //get path to backup
                string path = PathBuilder.GetToPath(name);

                //move backup to the bin folder and delete the previous backups
                Backup.Move(new DirectoryInfo(path), new DirectoryInfo(PathBuilder.GetBinBcpsFolderPath(name)));

            } catch(DirectoryNotFoundException) {
                return false;

            } finally {
                backuperFile.Delete();
            }

            return true;
        }

        private static List<string> GetBackupersNames() {
            var names = new List<string>();

            var directory = new DirectoryInfo(BackupersPath);

            if(!directory.Exists) {
                directory.Create();
                return new List<string>();
            }

            var files = directory.GetFiles();

            foreach(FileInfo file in files) {
                names.Add(Path.GetFileNameWithoutExtension(file.Name));
            }

            return names;
        }

        private static Backuper Load(string path) {
            string[] lines = File.ReadAllLines(path);
            return Backuper.Parse(lines);
        }
        #endregion private

    }
}
