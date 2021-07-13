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
using BackuperLibrary.ErrorHandling;

namespace BackuperLibrary {

    /// <summary>
    /// A class that focuses on backups related to a single source folder/file.
    /// </summary>
    public class Backuper {

        /// <summary>
        /// Initializes a new instance of <see cref="Backuper"/> and creates its directory if it's missing.
        /// </summary>
        /// <param name="name">The name of the backuper, <see cref="Name"/>.</param>
        /// <param name="source">A <see cref="FileSystemInfo"/> object that leads to the source folder, <see cref="Source"/>.</param>
        /// <param name="maxVersions">The maximum versions of backups allowed, <see cref="MaxVersions"/>.</param>
        /// <param name="updateAutomatically">Whether this backuper should execute a backup on windows' user's startup, <see cref="UpdateAutomatically"/>.</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Backuper(string name, FileSystemInfo source, int maxVersions, bool updateAutomatically) {

            if(!source.Exists) {
                throw new DirectoryNotFoundException("The source directory has not been found.");
            }
            if(maxVersions < 1) {
                throw new ArgumentOutOfRangeException("The maxVersions argument can't be lower than one.");
            }

            Source = source;
            Name = name;
            MaxVersions = maxVersions;
            UpdateAutomatically = updateAutomatically;

            //if the main folder hasn't been created, create it
            if(!Directory.Exists(To)) {
                Directory.CreateDirectory(To);
            }
        }

        /// <summary>
        /// Fires in case of a successful backup. It won't fire for failures, or if the latest backup is already updated.
        /// </summary>
        public event EventHandler BackupComplete;

        /// <summary>
        /// The name of this backuper. The name gets used as the folder's name for the backups.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Either a <see cref="DirectoryInfo"/> or <see cref="FileInfo"/> to make IO work with the source folder.
        /// </summary>
        public FileSystemInfo Source { get; private set; }

        /// <summary>
        /// The path to what this backuper backups.
        /// </summary>
        public string SourcePath { get => Source.FullName; }

        /// <summary>
        /// The path to this backuper's main backup folder.
        /// </summary>
        public string To { get => PathBuilder.GetToPath(this); }

        /// <summary>
        /// The maximum number of allowed backup versions. When the current exceeds this number, the oldest version gets deleted.
        /// </summary>
        public int MaxVersions { get; private set; }

        /// <summary>
        /// <see langword="True"/> if the latest folder is more recent than the <see cref="Source"/>'s folder; otherwise, <see langword="False"/>.
        /// </summary>
        public bool IsUpdated { get => IsLatest(); }

        /// <summary>
        /// Whether this backuper will execute a backup at the window user's start up.
        /// </summary>
        public bool UpdateAutomatically { get; set; }

        private readonly Locker locker = new Locker("This backuper is being used elsewhere.");

        /// <summary>
        /// Saves the backuper's parameters to its own config file.
        /// </summary>
        public void SaveToFile() {
            locker.LockHere(() => BackupersManager.Save(this));
        }

        /// <summary>
        /// Edits the backuper's parameters.
        /// </summary>
        /// <param name="newName">New name for the backuper—if this is not null, the files will be renamed/moved to fit the new name.</param>
        /// <param name="newMaxVersions">To change the current max versions amount.</param>
        /// <param name="updateAutomatically">To change if the backuper will backup on windows' user's start up.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="TimeoutException"></exception>
        public void Edit(string newName = null, int newMaxVersions = 0, bool? updateAutomatically = null) {
            locker.LockHere(() => {

                if(newName != null && newName != Name) {
                    if(BackupersManager.BackupersNames.Any(x => x == newName)) {
                        throw new ArgumentException("The name is already occupied.");
                    }

                    string pastName = Name;
                    string pastTo = To;
                    Name = newName;

                    Backup.Move(new DirectoryInfo(pastTo), new DirectoryInfo(To));

                    this.EditName(pastName, Name);
                }

                if(newMaxVersions > 1 && newMaxVersions != MaxVersions) {
                    MaxVersions = (int)newMaxVersions;
                    CleanUpExtraVersions();
                }
                
                if(updateAutomatically is not null) {
                    UpdateAutomatically = (bool)updateAutomatically;
                }

                BackupersManager.Save(this);
            });

        }

        /// <summary>
        /// Erases the backuper. Can be used to erase both backuper and its backups, or, if <paramref name="fullyDelete"/> is false, to delete the backuper and move the files to a bin folder.
        /// </summary>
        /// <param name="fullyDelete"><see langword="True"/> to delete all backups, <see langword="False"/> to move them to a bin folder.</param>
        /// <returns>An user-friendly message on whether the operation was a success.</returns>
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
                        string binPath = PathBuilder.GetBinBcpsFolderPath(this);
                        Backup.CopyAndPaste(new DirectoryInfo(To), new DirectoryInfo(binPath));
                        var directory = new DirectoryInfo(To);
                        directory.Delete(true);
                        this.Delete();
                        return $"{Name} has been deleted, but the backups have been moved to backup folder [{binPath}].";
                    }
                } catch (Exception ex) {
                    Log.WriteError(ex);
                    return $"There was an error: {Environment.NewLine} {ex.Message}";
                }
            });

        }

        /// <summary>
        /// Executes a backup by using the <see cref="Backuper"/>'s parameters. Throws <see cref="TimeoutException"/> if the backuper is already being used elsewhere.
        /// </summary>
        /// <returns>A <see cref="BackuperResultInfo"/> to return the resulst of the backup.</returns>
        /// <exception cref="TimeoutException"></exception>
        public BackuperResultInfo MakeBackup() {
            return locker.LockHere(() => {

                if(IsUpdated) {
                    return Factory.CreateBackupResult(Name, BackuperResult.AlreadyUpdated);
                }

                try {

                    ActBackup();

                    //this try-catch is to prevent odd exceptions if the user closes the app before a backup is completed
                    try {
                        BackupComplete?.Invoke(this, EventArgs.Empty);
                    } catch (TaskCanceledException) { }

                    return Factory.CreateBackupResult(Name, BackuperResult.Success);
                } catch (Exception ex) {
                    Log.WriteError(ex);
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
            return $"{Name},{Source.FullName},{MaxVersions},{UpdateAutomatically}";
        }

        /// <summary>
        /// Returns a string that shows the backuper's <paramref name="Name"/>, <paramref name="From"/>, and <paramref name="MaxVersions"/> divided by a custom separator.
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string ToString(string separator) {
            return $"{Name}{separator}{Source.FullName}{separator}{MaxVersions}{separator}{UpdateAutomatically}";
        }

        /// <summary>
        /// Converts a string into a <see cref="Backuper"/>. <br/><br/>Proper Format: <br/><see cref="Name"/>,<see cref="SourcePath"/>,<see cref="MaxVersions"/>,<see cref="UpdateAutomatically"/>
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>An instantiated backuper equivalent to the given <paramref name="s"/> string.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public static Backuper Parse(string s) {
            var lines = s.Split(',');

            return Parse(lines);
        }

        /// <summary>
        /// Converts a string into a <see cref="Backuper"/>. <br/><br/>Proper Format: <br/><see cref="Name"/>\r\n<see cref="SourcePath"/>\r\n<see cref="MaxVersions"/>\r\n<see cref="UpdateAutomatically"/>
        /// </summary>
        /// <param name="lines">The string to convert.</param>
        /// <returns>An instantiated backuper equivalent to the given <paramref name="lines"/> list.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public static Backuper Parse(string[] lines) {
            return Factory.CreateBackuper(lines[0], lines[1], int.Parse(lines[2]), bool.Parse(lines[3]));
        }

        #endregion conversions

        #region private

        /// <summary>
        /// Logic to execute the backup.
        /// </summary>
        private void ActBackup() {
            //setup necessary stuff
            string date = $"{PathBuilder.ChangeFormat(DateTime.Now.ToShortDateString())} - {PathBuilder.ChangeFormat(DateTime.Now.ToLongTimeString())}";
            string path = Path.Combine(To, date);

            //create new folder to hold the new backup
            Directory.CreateDirectory(path);

            //copy "from" to the new folder
            Backup.CopyAndPaste(Source, new DirectoryInfo(path));

            //check if there are too many versions and, if there are, delete the oldest ones
            CleanUpExtraVersions();
        }

        /// <summary>
        /// Checks whether there are more versions than what <see cref="MaxVersions"/> allows. If there are, deletes the extra, oldest ones.
        /// </summary>
        private void CleanUpExtraVersions() {
            var versions = Directory.GetDirectories(To);
            List<string> orderedVersions = versions.OrderBy(x => Directory.GetCreationTime(x)).ToList();

            while(orderedVersions.Count() > MaxVersions) {
                var directory = new DirectoryInfo(orderedVersions.First());
                directory.Delete(true);
                orderedVersions.Remove(orderedVersions.First());
            }
        }

        /// <summary>
        /// Compares the newest backup's and the source folder's last write time to determine if the backup is more recent.
        /// </summary>
        /// <returns><see langword="True"/> if the newest backup folder is more recent than the source's; otherwise, <see langword="False"/></returns>
        private bool IsLatest() {
            string latestVersionPath = Comparer.GetLatestVersion(new DirectoryInfo(To));

            if(latestVersionPath == null) {
                return false;
            }

            return Directory.GetLastWriteTime(Source.FullName) < Directory.GetLastWriteTime(latestVersionPath);
        }
        #endregion private

    }
}
