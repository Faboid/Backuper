﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BackuperLibrary {
    public class Backuper {

        public Backuper(string from, string to, string name, int maxVersions) {

            if(!Directory.Exists(from)) {
                throw new DirectoryNotFoundException("The source directory has not been found.");
            }
            if(maxVersions < 1) {
                throw new ArgumentException("The maxVersions argument can't be lower than one.");
            }


            From = from;
            To = to;
            Name = name;
            MaxVersions = maxVersions;

            //if the main folder hasn't been created, create it
            if(!Directory.Exists(to)) {
                Directory.CreateDirectory(to);
            }
        }

        public string Name { get; private set; }
        public string From { get; private set; }

        //todo - centralize all "To" to a general path, then build unique paths through the use of "Name"
        public string To { get; private set; }
        public int MaxVersions { get; private set; }
        public bool IsUpdated { get => IsLatest(); }

        public string MakeBackup() {
            if(IsUpdated) {
                return "The backup's version is already updated.";
            }

            try {
                ActBackup();
                return "The backup has been completed successfully.";
            } catch (Exception ex) {
                return $"There was an error: {Environment.NewLine} {ex.Message}";
            }
        }

        private void ActBackup() {
            //setup necessary stuff
            string date = $"{ChangeFormat(DateTime.Now.ToShortDateString())} - {ChangeFormat(DateTime.Now.ToLongTimeString())}";
            string path = Path.Combine(To, date);

            //create new folder to hold the new backup
            Directory.CreateDirectory(path);

            //copy "from" to the new folder
            Backup.CopyAndPaste(new DirectoryInfo(From), new DirectoryInfo(path));
        }

        private static string ChangeFormat(string input) {
            StringBuilder sb = new StringBuilder(input.Length);

            for(int i = 0; i < input.Length; i++) {
                if(input[i] == '/') {
                    sb.Append('-');
                } else if(input[i] == ':') {
                    sb.Append(',');
                } else {
                    sb.Append(input[i]);
                }
            }

            return sb.ToString();
        }

        private bool IsLatest() {
            string latestVersionPath = Comparer.GetLatestVersion(To);

            if(latestVersionPath == null) {
                return false;
            }

            return Directory.GetLastWriteTime(From) <= Directory.GetLastWriteTime(latestVersionPath);
        } 

        public override string ToString() {
            throw new NotImplementedException();
        }

        public static bool TryParse(string s, out Backuper result) {
            throw new NotImplementedException();
        }
    }
}
