using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackuperLibrary.UISpeaker;

namespace BackuperLibrary.Generic {
    public static class Factory {

        public static BackuperResultInfo CreateBackupResult(string nameBackup, BackuperResult result, Exception ex = null) {
            return new BackuperResultInfo(nameBackup, result, ex);
        }

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
