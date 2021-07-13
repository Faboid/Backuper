using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackuperLibrary.UISpeaker;

namespace BackuperLibrary.Generic {
    public static class Factory {

        /// <summary>
        /// Creates a <see cref="BackuperResultInfo"/> instance to encapsulate the result of a backup
        /// </summary>
        /// <param name="nameBackup">The name of the backuper</param>
        /// <param name="result">The result of the backup</param>
        /// <param name="ex">If an unhandled exception occurred, it can be relayed to the UI through this</param>
        /// <returns>An initiated instance of <see cref="BackuperResultInfo"/></returns>
        public static BackuperResultInfo CreateBackupResult(string nameBackup, BackuperResult result, Exception ex = null) {
            return new BackuperResultInfo(nameBackup, result, ex);
        }

        /// <summary>
        /// Checks whether the given <paramref name="sourcePath"/> leads to a directory or a file, then creates a <see cref="Backuper"/> accordingly.
        /// </summary>
        /// <param name="name">The name of the backuper.</param>
        /// <param name="sourcePath">The path to the backuper's source.</param>
        /// <param name="maxVersions">The maximum number of versions. When a backup exceeds the amount, the oldest versions are deleted.</param>
        /// <param name="updateAutomatically">Whether this backuper will be updated at windows' user's start up.</param>
        /// <returns>An instantiated <see cref="Backuper"/></returns>
        /// <exception cref="InvalidDataException">If the source path doesn't lead to a file or folder.</exception>
        public static Backuper CreateBackuper(string name, string sourcePath, int maxVersions, bool updateAutomatically) {

            if(Directory.Exists(sourcePath)) {
                return new Backuper(name, new DirectoryInfo(sourcePath), maxVersions, updateAutomatically);

            } else if (File.Exists(sourcePath)) {
                return new Backuper(name, new FileInfo(sourcePath), maxVersions, updateAutomatically);

            } else {
                throw new InvalidDataException($"The source path doesn't exist or is invalid: {sourcePath}");
            }

        }

    }
}
