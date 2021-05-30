using BackuperLibrary.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackuperLibrary.IO {
    public static class BackupFolderHandler {

        static BackupFolderHandler() {
            if(!File.Exists(ConfigFilePath)) {
                File.WriteAllText(ConfigFilePath, DefaultTo);
            }
            RefreshToPath();
        }

        internal static string To { get; private set; }

        private static string DefaultTo { get; } = Path.Combine(PathBuilder.GetWorkingDirectory(), "Backups");

        private static string ConfigFilePath = Path.Combine(PathBuilder.GetWorkingDirectory(), "Settings.txt");

        public static bool SetDefault() {
            if(To == DefaultTo) {
                return true;
            }

            if(!Directory.Exists(DefaultTo)) {
                Directory.CreateDirectory(DefaultTo);
            }

            return TryChangePath(DefaultTo);
        }

        private static void RefreshToPath() {
            To = File.ReadAllText(ConfigFilePath);
        }

        public static bool TryChangePath(string newPath) {
            DirectoryInfo info = new DirectoryInfo(newPath);
            if(info.GetFiles().Length > 0 || info.GetDirectories().Length > 0) {
                throw new ArgumentException("To sustain the correct functioning of the application, it's necessary to choose an empty location for the backups.");
            }

            if(Directory.Exists(newPath)) {
                var moveFrom = new DirectoryInfo(To);
                var moveTo = new DirectoryInfo(newPath);
                Backup.Move(moveFrom, moveTo);

                File.WriteAllText(ConfigFilePath, newPath);
                RefreshToPath();

                return true;
            }

            return false;
        }

    }
}
