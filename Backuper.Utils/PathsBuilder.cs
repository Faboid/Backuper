﻿using System.Globalization;

namespace Backuper.Utils {
    public class PathsBuilder {

        //todo - implement a way of saving a new default path
        public PathsBuilder() : this(Directory.GetCurrentDirectory()) { }

        /// <summary>
        /// Creates a new <see cref="PathsBuilder"/> with a custom path. 
        /// Throws <see cref="ArgumentException"/> if the directory doesn't exist.
        /// </summary>
        /// <param name="customMainDirectory">Path to an existing directory to use as a root for the paths.</param>
        /// <exception cref="ArgumentException"></exception>
        internal PathsBuilder(string customMainDirectory) {
            if(!Directory.Exists(customMainDirectory)) {
                throw new ArgumentException("The given directory does not exist.", nameof(customMainDirectory));
            }

            BackupsMainDirectory = customMainDirectory;
        }

        private string BackupsMainDirectory { get; }
        
        public Paths Build(string backuperName) {
            return new(BackupsMainDirectory, backuperName);
        }

    }

    public class Paths {

        //path: dataDirectory/Backuper/Backups/BackuperName/Version
        //path: dataDirectory/Backuper/Bin/BackuperName/Version

        public Paths(string dataDirectory, string backuperName) {
            dataDirectory = Path.Combine(dataDirectory, "Backuper");
            BackupsDirectory = Path.Combine(dataDirectory, "Backups", backuperName);
            BinDirectory = Path.Combine(dataDirectory, "Bin", backuperName);
        }

        public string BinDirectory { get; }
        public string BackupsDirectory { get; }

        public string GenerateNewBackupVersionDirectory() {
            return GenerateNewBackupVersionDirectory(DateTime.Now);
        }

        internal string GenerateNewBackupVersionDirectory(DateTime dateTime) {
            DateTimeFormatInfo format = new();
            var dateAsString = dateTime.ToString(format.UniversalSortableDateTimePattern);
            return Path.Combine(BackupsDirectory, dateAsString);
        }

    }

}
