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

        /// <summary>
        /// Gets the path to the main backup folder, where all backups are stored.
        /// </summary>
        internal static string To { get; private set; }

        private static string DefaultTo { get; } = Path.Combine(PathBuilder.GetWorkingDirectory(), "Backups");

        private static string ConfigFilePath = Path.Combine(PathBuilder.GetWorkingDirectory(), "Settings.txt");

        /// <summary>
        /// Resets the current <see cref="To"/> path to the default.
        /// </summary>
        /// <returns><see langword="True"/> if it reset successfully; otherwise, <see langword="False"/>.</returns>
        /// <exception cref="IOException"></exception>
        public static bool SetDefault() {
            if(To == DefaultTo) {
                return true;
            }

            if(!Directory.Exists(DefaultTo)) {
                Directory.CreateDirectory(DefaultTo);
            }

            return TryChangePath(new DirectoryInfo(DefaultTo));
        }

        /// <summary>
        /// Refreshes <see cref="To"/>'s path by taking it from its save file in <see cref="ConfigFilePath"/>.
        /// </summary>
        private static void RefreshToPath() {
            To = File.ReadAllText(ConfigFilePath);
        }

        /// <summary>
        /// Tries to change <see cref="To"/> path to a new one.
        /// </summary>
        /// <param name="newPath">The new path.</param>
        /// <returns><see langword="True"/> if it reset successfully; otherwise, <see langword="False"/>.</returns>
        /// <exception cref="IOException"></exception>
        public static bool TryChangePath(DirectoryInfo newPath) {
            if(newPath.GetFiles().Length > 0 || newPath.GetDirectories().Length > 0) {
                throw new IOException("To sustain the correct functioning of the application, it's necessary to choose an empty location for the backups.");
            }

            if(newPath.Exists) {

                return Settings.SetThreadForegroundHere(() => {
                    var moveFrom = new DirectoryInfo(To);
                    Backup.Move(moveFrom, newPath);

                    File.WriteAllText(ConfigFilePath, newPath.FullName);
                    RefreshToPath();

                    return true;
                });
            }

            return false;
        }

    }
}
