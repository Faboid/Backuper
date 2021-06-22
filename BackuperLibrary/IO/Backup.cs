﻿using BackuperLibrary.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackuperLibrary.IO {
    public static class Backup {

        public static void Move(DirectoryInfo from, DirectoryInfo to) {

            Settings.SetThreadForegroundHere(() => {

                //create directory to move the backups
                to.Create();

                //copy all past backups to new location
                CopyAndPaste(from, to);

                //delete past location
                from.Delete(true);
            });
        }

        public static void CopyAndPaste(FileSystemInfo from, DirectoryInfo to) {

            Settings.SetThreadForegroundHere(() => {
                
                if(from is FileInfo) {
                    FileInfo destination = new FileInfo($"{to.FullName}\\{from.Name}");
                    File.Copy(from.FullName, destination.FullName);

                    return;
                }


                //get all directories
                var directories = GetAllDirectories(from as DirectoryInfo);
                //get all files
                var files = GetAllFiles(directories);

                //create all the needed directories
                CreateAllDirectories(directories, from.FullName, to.FullName);
                //copy the files to the new location
                CreateAllFiles(files, from.FullName, to.FullName);
            });

        }

        #region private
        private static void CreateAllDirectories(List<DirectoryInfo> source, string from, string to) {
            foreach(DirectoryInfo directoryFrom in source) {
                DirectoryInfo directoryTo = new DirectoryInfo(directoryFrom.FullName.Replace(from, to));

                Directory.CreateDirectory(directoryTo.FullName);
            }
        }

        private static void CreateAllFiles(List<FileInfo> source, string from, string to) {
            foreach(FileInfo fileFrom in source) {
                FileInfo fileTo = new FileInfo(fileFrom.FullName.Replace(from, to));

                File.Copy(fileFrom.FullName, fileTo.FullName);
            }
        }

        private static List<DirectoryInfo> GetAllDirectories(DirectoryInfo source) {
            List<DirectoryInfo> allDirectories = new List<DirectoryInfo>();
            allDirectories.Add(source);
            var directories = source.GetDirectories();

            foreach(DirectoryInfo directory in directories) {
                allDirectories.AddRange(GetAllDirectories(directory));
            }

            return allDirectories;
        }

        private static List<FileInfo> GetAllFiles(List<DirectoryInfo> source) {
            List<FileInfo> files = new List<FileInfo>();

            foreach(DirectoryInfo directory in source) {
                files.AddRange(directory.GetFiles());
            }

            return files;
        }

        #endregion private
    }
}
