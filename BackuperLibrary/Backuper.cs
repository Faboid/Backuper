using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BackuperLibrary.IO;
using BackuperLibrary.UISpeaker;
using BackuperLibrary.Generic;
using System.Threading;
using BackuperLibrary.Safety;

namespace BackuperLibrary {
    public class Backuper {

        public Backuper(string name, string from, int maxVersions) {

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

        public event EventHandler BackupComplete;

        public string Name { get; private set; }
        public string From { get; private set; }
        public string To { get => PathBuilder.GetToPath(Name); }
        public int MaxVersions { get; private set; }
        public bool IsUpdated { get => IsLatest(); }
        private readonly Locker locker = new Locker();

        public void SaveToFile() {
            locker.LockHere(() => BackupersManager.Save(this));
        }

        public void Edit(string newName = null, int newMaxVersions = 0) {
            locker.LockHere(() => {

                if(newMaxVersions > 1 && newMaxVersions != MaxVersions) {
                    MaxVersions = (int)newMaxVersions;
                    CleanUpExtraVersions();
                    BackupersManager.Save(this);
                }

                if(newName != null && newName != Name) {
                    if(BackupersManager.BackupersNames.Any(x => x == newName)) {
                        throw new ArgumentException("The name is already occupied.");
                    }

                    string pastName = Name;
                    string pastTo = To;
                    Name = newName;

                    Backup.Move(new DirectoryInfo(pastTo), new DirectoryInfo(To));

                    BackupersManager.EditName(this, pastName, Name);
                }
            });

        }

        public string Erase(bool fullyDelete) {
            //True = delete all backups. False = Move backups to a special "bin" folder

            return locker.LockHere(() => {

                try {
                    if(fullyDelete) {
                        var directory = new DirectoryInfo(To);
                        directory.Delete(true);
                        this.Delete();
                        return $"The backups of {Name} have been deleted successfully.";
                    } else {
                        //move to /Bin/ folder
                        string binPath = PathBuilder.GetBinBcpsFolderPath(Name);
                        Backup.CopyAndPaste(new DirectoryInfo(To), new DirectoryInfo(binPath));
                        var directory = new DirectoryInfo(To);
                        directory.Delete(true);
                        this.Delete();
                        return $"{Name} has been deleted, but the backups have been moved to backup folder [{binPath}].";
                    }
                } catch (Exception ex) {
                    return $"There was an error: {Environment.NewLine} {ex.Message}";
                }
            });

        }

        public BackuperResultInfo MakeBackup() {
            return locker.LockHere(() => {

                if(IsUpdated) {
                    return Factory.CreateBackupResult(Name, BackuperResult.AlreadyUpdated);
                }

                try {

                    ActBackup();
                    try {
                        BackupComplete?.Invoke(this, EventArgs.Empty);
                    } catch (TaskCanceledException) { }

                    return Factory.CreateBackupResult(Name, BackuperResult.Success);
                } catch (Exception ex) {

                    return Factory.CreateBackupResult(Name, BackuperResult.Failure, ex);
                }
            });

        }

        #region Locking

        /// <summary>
        /// Checks whether the lock is free.
        /// </summary>
        /// <returns><see langword="true"/> if the lock is open; otherwise, <see langword="false"/>.</returns>
        public bool CheckLock() => locker.CheckLock();

        #endregion Locking

        #region conversions
        public override string ToString() {
            return $"{Name},{From},{MaxVersions}";
        }

        /// <summary>
        /// Returns a string that shows the backuper's <paramref name="Name"/>, <paramref name="From"/>, and <paramref name="MaxVersions"/> divided by a custom separator.
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string ToString(string separator) {
            return $"{Name}{separator}{From}{separator}{MaxVersions}";
        }

        public static Backuper Parse(string s) {
            var lines = s.Split(',');

            return new Backuper(lines[0], lines[1], int.Parse(lines[2]));
        }

        public static Backuper Parse(string[] lines) {
            return new Backuper(lines[0], lines[1], int.Parse(lines[2]));
        }

        public static bool TryParse(string s, out Backuper result) {
            throw new NotImplementedException();
        }
        #endregion conversions

        #region private
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

            return Directory.GetLastWriteTime(From) < Directory.GetLastWriteTime(latestVersionPath);
        }
        #endregion private

    }
}
