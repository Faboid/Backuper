using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackuperLibrary {
    public static class Backup {

        static private object locker;

        public static void NewBackup(string from, string to, string name, int maxVersions) {

            lock(locker) {
                if(!AreDifferent(from, GetLatestVersion(to))) {
                    ActBackup(from, to, name, maxVersions);
                }
            }
            //NotifyUserAboutCompletion();
        }

        public static bool AreDifferent(string from, string to) {
            throw new NotImplementedException();
        }

        public static void ActBackup(string from, string to, string name, int maxVersions) {
            throw new NotImplementedException();

            //string newTo = CreateNewBackupVersion(to, name, DateTime.Now);
            //CopyDirectory(from, newTo);
            //DeleteOldVersionsIfTooMany(to, maxVersions);
        }

        public static string GetLatestVersion(string to) {
            throw new NotImplementedException();

            //iterates through all directories(versions) and get the most recent one
        }

    }
}
