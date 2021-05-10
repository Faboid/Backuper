using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BackuperLibrary {
    public static class BackupersHandler {

        //folder that contains the path to the "To" folder
        private static string ToFolderPath { get; } = @$"{GetWorkingDirectory()}\Paths.txt";

        //folder that contains all the strings relative to the backupers
        private static string BackupersPath { get; } = @$"{GetWorkingDirectory()}\Backupers.txt";


        public static List<Backuper> Backupers { get; private set; } = LoadBackupers();
        private static string GetWorkingDirectory() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void AddBackuper(Backuper backuper) {
            Backupers.Add(backuper);
            SaveBackupersToFile();
        }

        public static void ModifyBackuper(Backuper backuper, string newName = null, int newMaxVersions = 0) {
            backuper.ModifyBackuper(newName, newMaxVersions);

            SaveBackupersToFile();
        }

        public static void DeleteBackuper(string name, string sourcePath, bool deleteSavedBackups, out string message) {
            //get specified backuper
            var backuper = Backupers.Where(x => x.Name == name && x.From == sourcePath).Single();

            if(deleteSavedBackups) {
                //erase backups if true
                message = backuper.EraseBackups();
            } else {
                //todo - if false, extract to "DELETED" folder
                message = $"The previous backups of {name} have been moved to path - to implement";

                //temporary message until the "extract" feature is implemented
                message = "The element has been deleted, but the already-made backups have been left in the backup folder.";
            }

            //remove from list
            Backupers.Remove(backuper);

            SaveBackupersToFile();
        }

        private static List<Backuper> LoadBackupers() {
            //if file doesn't exist, create an empty one
            if(!File.Exists(BackupersPath)) {
                File.Create(BackupersPath);
                return new List<Backuper>();
            }

            //load config file
            string[] backupersStrings = File.ReadAllLines(BackupersPath);

            //convert strings to backupers and return
            return backupersStrings.Select(x => Backuper.Parse(x)).ToList();
        }

        private static void SaveBackupersToFile() {
            File.WriteAllLines(BackupersPath, Backupers.Select(x => x.ToString()));
        }


        private static List<Backuper> GetSampleBackupers() {
            var backupers = new List<Backuper>();
            backupers.Add(new Backuper(@"D:\Programming\Small Projects\Backuper\TemporaryTestFolder\From", "Test", 5));
            backupers.Add(new Backuper(@"D:\Programming\Small Projects\Backuper\TemporaryTestFolder\From", "SecondTest", 5));
            backupers.Add(new Backuper(@"D:\Programming\Small Projects\Backuper\TemporaryTestFolder\From", "ThirdTest", 5));
            return backupers;
        }

    }
}
