using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BackuperLibrary.Generic;

namespace BackuperLibrary {
    public class Backuper {

        public Backuper(string from, string name, int maxVersions) {

            if(!Directory.Exists(from)) {
                throw new DirectoryNotFoundException("The source directory has not been found.");
            }
            if(maxVersions < 1) {
                throw new ArgumentException("The maxVersions argument can't be lower than one.");
            }

            From = from;
            Name = name;
            MaxVersions = maxVersions;

            //if the main folder hasn't been created, create it
            if(!Directory.Exists(To)) {
                Directory.CreateDirectory(To);
            }
        }

        public string Name { get; private set; }
        public string From { get; private set; }
        public string To { get => PathBuilder.GetToPath(Name); }
        public int MaxVersions { get; private set; }
        public bool IsUpdated { get => IsLatest(); }

        public void ModifyBackuper(string newName = null, int newMaxVersions = 0) {
            if(newMaxVersions > 1 && newMaxVersions != MaxVersions) {
                MaxVersions = (int)newMaxVersions;
                CleanUpExtraVersions();
            }

            if(newName != null && newName != Name) {
                if(BackupersHandler.Backupers.Any(x => x.Name == newName)) {
                    throw new ArgumentException("The name is already occupied.");
                }

                string pastTo = To;
                Name = newName;

                //create directory to move the backups
                Directory.CreateDirectory(To);

                //copy all past backups to new location
                Backup.CopyAndPaste(new DirectoryInfo(pastTo), new DirectoryInfo(To));

                //delete past location
                Directory.Delete(pastTo, true);
            }
        }

        public string EraseBackups() {
            //todo - add boolean parameter. True = delete all. False = Move backups to a special "bin" folder
            try {
                var directory = new DirectoryInfo(To);
                directory.Delete(true);
                return $"The backups of {Name} have been deleted successfully.";
            } catch (Exception ex) {
                return $"There was an error: {Environment.NewLine} {ex.Message}";
            }
        }

        public BackuperResultInfo MakeBackup() {
            if(IsUpdated) {
                return Factory.CreateBackupResult(Name, BackuperResult.AlreadyUpdated);
            }

            try {
                ActBackup();

                return Factory.CreateBackupResult(Name, BackuperResult.Success);

            } catch (Exception ex) {

                return Factory.CreateBackupResult(Name, BackuperResult.Failure, ex);
            }
        }

        private void ActBackup() {
            //setup necessary stuff
            string date = $"{PathBuilder.ChangeFormat(DateTime.Now.ToShortDateString())} - {PathBuilder.ChangeFormat(DateTime.Now.ToLongTimeString())}";
            string path = Path.Combine(To, date);

            //create new folder to hold the new backup
            Directory.CreateDirectory(path);

            //copy "from" to the new folder
            Backup.CopyAndPaste(new DirectoryInfo(From), new DirectoryInfo(path));

            //check if there are too many versions and, if there are, delete the oldest ones
            CleanUpExtraVersions();
        }

        private void CleanUpExtraVersions() {
            var versions = Directory.GetDirectories(To);
            List<string> orderedVersions = versions.OrderBy(x => Directory.GetCreationTime(x)).ToList();

            while(orderedVersions.Count() > MaxVersions) {
                var directory = new DirectoryInfo(orderedVersions.First());
                directory.Delete(true);
                orderedVersions.Remove(orderedVersions.First());
            }
        }

        private bool IsLatest() {
            string latestVersionPath = Comparer.GetLatestVersion(To);

            if(latestVersionPath == null) {
                return false;
            }

            return Directory.GetLastWriteTime(From) <= Directory.GetLastWriteTime(latestVersionPath);
        }

        #region conversions
        public override string ToString() {
            return $"{From},{Name},{MaxVersions}";
        }

        public static Backuper Parse(string s) {
            var lines = s.Split(',');

            return new Backuper(lines[0], lines[1], int.Parse(lines[2]));
        }

        public static bool TryParse(string s, out Backuper result) {
            throw new NotImplementedException();
        }
        #endregion conversions
    }
}
