using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BackuperLibrary.IO;

namespace BackuperLibrary {
    public static class BackupersHandler {

        //folder that contains the path to the "To" folder
        private static string ToFolderPath { get; } = @$"{GetWorkingDirectory()}\Paths.txt";

        //folder that contains all the strings relative to the backupers
        private static string BackupersPath { get; } = @$"{GetWorkingDirectory()}\Backupers.txt";


        public static List<Backuper> Backupers { get; private set; } = BackupersManager.LoadAll();
        private static string GetWorkingDirectory() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void AddBackuper(Backuper backuper) {
            Backupers.Add(backuper);
            BackupersManager.Save(backuper);
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
        }

    }
}
