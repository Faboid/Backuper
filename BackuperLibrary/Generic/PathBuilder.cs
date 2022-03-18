using BackuperLibrary.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackuperLibrary.Generic {
    //todo - split class in multiple classes: specifically, divide the "to" and the "bin" sections
    public static class PathBuilder {

        /// <summary>
        /// Gets the path to the main backup folder, where all backups are stored.
        /// </summary>
        public static string To { get => BackupFolderHandler.To; }

        /// <summary>
        /// The name of the bin, or the place where deleted backups can be stored.
        /// </summary>
        public static string BinName { get; } = "Bin";

        /// <summary>
        /// Gets the full path to the bin folder, where deleted backups can be stored.
        /// </summary>
        public static string BinBackupsFolder { get; } = Path.Combine(To, BinName);

        /// <summary>
        /// Gets the directory where the application's exe has been executed in.
        /// </summary>
        /// <returns>A string with the path to the exe's folder.</returns>
        public static string GetWorkingDirectory() => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        /// <summary>
        /// Combines <see cref="To"/> and <see cref="Backuper.Name"/> to get a path to <paramref name="backuper"/>'s backups location.
        /// </summary>
        /// <param name="backuper">The backuper whose name will be combined.</param>
        /// <returns>A string path to <paramref name="backuper"/>'s backups location.</returns>
        public static string GetToPath(Backuper backuper) => Path.Combine(To, backuper.Name);

        /// <summary>
        /// Combines <see cref="To"/> and <paramref name="backuperName"/> to get a path to a backuper's backups location. Throws <see cref="DirectoryNotFoundException"/> if the backup folder doesn't exist.
        /// </summary>
        /// <param name="backuperName">The name of the backuper.</param>
        /// <returns>A string path to the backuper's backups location.</returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static string GetToPath(string backuperName) { 
            string path = Path.Combine(To, backuperName); 
            if(Directory.Exists(path)) {
                return path;
            } else {
                throw new DirectoryNotFoundException("Couldn't find a backup folder. Is the given backuperName correct?");
            }
        }

        /// <summary>
        /// Combines <see cref="BinBackupsFolder"/> and <see cref="Backuper.Name"/> to get a path to <paramref name="backuper"/>'s bin location. In case the path is already occupied, adds a number at the end.
        /// </summary>
        /// <param name="backuper">The backuper whose name will be combined.</param>
        /// <returns>A string path to <paramref name="backuper"/>'s bin location.</returns>
        public static string GetBinBcpsFolderPath(Backuper backuper) => BinDuplicateProtection(backuper.Name);

        /// <summary>
        /// Combines <see cref="BinBackupsFolder"/> and <paramref name="backuperName"/> to get a path to the backuper's bin location. In case the path is already occupied, adds a number at the end.
        /// </summary>
        /// <param name="backuperName">The backuper whose name will be combined.</param>
        /// <returns>A string path to the backuper's bin location.</returns>
        public static string GetBinBcpsFolderPath(string backuperName) => BinDuplicateProtection(backuperName);


        /// <summary>
        /// Changes a datatime's string format into one that can be used as a folder's name(it changes forbidden characters into allowed ones).
        /// </summary>
        /// <param name="input">The string of the datatime.</param>
        /// <returns>A string that can be used as a folder's name.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ChangeFormat(string input) {

            var sb = new StringBuilder(input.Length);

            for(int i = 0; i < input.Length; i++) {
                if(input[i] == '/') {
                    sb.Append('-');
                } else if(input[i] == ':') {
                    sb.Append(',');
                } else {
                    sb.Append(input[i]);
                }
            }

            if(!ValidatePath(sb.ToString(), out _)) {
                throw new ArgumentException("The given input is invalid even after it has been checked. Note: this function only accepts strings derived from DateTime.", paramName: input);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Checks the given string for forbidden characters(those that cannot be used in windows' paths).
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <param name="message">An user-friendly message that can be displayed in case the tests fails.</param>
        /// <returns><see langword="True"/> if the path lacks any forbidden character; otherwise, <see langword="false"/>.</returns>
        public static bool ValidatePath(string path, out string message) {
            message = "";
            // invalid characters: \ / : * ? " < > | 
            char[] invalidCharacters = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

            if(path.Any(x => invalidCharacters.Contains(x))) {
                message = "The path cannot contain any of the following characters: \\ / : * ? \" < > |";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Searches for a version of the backuper's bin path that isn't already occupied by an existing folder; adds a number at the end to get new versions.
        /// </summary>
        /// <param name="backuperName">The name of the backuper.</param>
        /// <returns>A path to a non-existing bin.</returns>
        private static string BinDuplicateProtection(string backuperName) {
            string path = GetBinNameFolder(backuperName);

            //if there is no duplicate, return
            if(!Directory.Exists(path)) {
                return path;
            }

            //change name with a numberical addiction until a non-existing one is found
            int number = 1;
            while(Directory.Exists(GetBinNameFolder($"{backuperName}({number})"))) {
                number++;
            }

            return Path.Combine(BinBackupsFolder, $"{backuperName}{number}");
        }

        /// <summary>
        /// Combines <see cref="BinBackupsFolder"/> and <paramref name="backuperName"/> to get a path to the backuper's bin location.
        /// </summary>
        /// <param name="backuperName">The backuper whose name will be combined.</param>
        /// <returns>A string path to the backuper's bin location.</returns>
        private static string GetBinNameFolder(string backuperName) => Path.Combine(BinBackupsFolder, backuperName);
    }
}
