using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using BackuperLibrary.Generic;

namespace BackuperLibrary.IO {
    public static class BackupersManager {

        public static event EventHandler EditedBackupers;
        public static string BackupersPath { get; } = @$"{PathBuilder.GetWorkingDirectory()}\Backupers";
        public static List<string> BackupersNames { get; private set; } = GetBackupersNames();


        static BackupersManager() {
            EditedBackupers += RefreshBackupersNames;
        }


        public static void SaveAll(this List<Backuper> backupers) {
            backupers.ForEach(x => Save(x));
            EditedBackupers?.Invoke(null, EventArgs.Empty);
        }

        public static void Save(this Backuper backuper) {
            string path = GetBackuperPath(backuper.Name);
            string content = backuper.ToString();
            string[] lines = content.Split(',');
            File.WriteAllLines(path, lines);
            EditedBackupers?.Invoke(null, EventArgs.Empty);
        }

        public static void EditName(Backuper sender, string pastName, string newName) {
            string pastPath = GetBackuperPath(pastName);
            string newPath = GetBackuperPath(newName);
            File.Copy(pastPath, newPath);
            File.Delete(pastPath);
            Save(sender);
            EditedBackupers?.Invoke(null, EventArgs.Empty);
        }

        public static void Delete(this Backuper backuper) {
            string path = GetBackuperPath(backuper.Name);
            File.Delete(path);
            EditedBackupers?.Invoke(null, EventArgs.Empty);
        }

        public static List<Backuper> LoadAll() {
            List<Backuper> backupers = new List<Backuper>();

            var directory = new DirectoryInfo(BackupersPath);

            if(!directory.Exists) {
                directory.Create();
                return backupers;
            }

            var files = directory.GetFiles();

            foreach(FileInfo file in files) {
                backupers.Add(Load(file.FullName));
            }

            return backupers;
        }

        #region private

        private static string GetBackuperPath(string name) => Path.Combine(BackupersPath, $"{name}.txt");

        private static void RefreshBackupersNames(object sender, EventArgs e) {
            BackupersNames = GetBackupersNames();
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
