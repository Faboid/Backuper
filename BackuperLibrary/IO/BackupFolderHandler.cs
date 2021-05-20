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

        private static void RefreshToPath() {
            To = File.ReadAllText(ConfigFilePath);
        }

        public static void TryChangePath(string newPath) {
            if(Directory.Exists(newPath)) {
                File.WriteAllText(ConfigFilePath, newPath);
                RefreshToPath();
            }
        }

    }
}
