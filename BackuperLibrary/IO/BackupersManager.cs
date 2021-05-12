using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace BackuperLibrary.IO {
    public static class BackupersManager {
        private static string GetWorkingDirectory() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string BackupersPath { get; } = @$"{GetWorkingDirectory()}\Backupers";
        private static string GetBackuperPath(string name) => Path.Combine(BackupersPath, $"{name}.txt");

        public static void SaveAll(this List<Backuper> backupers) {
            backupers.ForEach(x => Save(x));
        }

        public static void Save(this Backuper backuper) {
            string path = GetBackuperPath(backuper.Name);
            string content = backuper.ToString();
            string[] lines = content.Split(',');
            File.WriteAllLines(path, lines);
        }

        public static void EditName(string pastName, string newName) {
            string pastPath = GetBackuperPath(pastName);
            string newPath = GetBackuperPath(newName);
            File.Copy(pastPath, newPath);
            File.Delete(pastPath);
        }

        public static void Delete(this Backuper backuper) {
            string path = GetBackuperPath(backuper.Name);
            File.Delete(path);
        }

        public static List<Backuper> LoadAll() {
            List<Backuper> backupers = new List<Backuper>();

            var directory = new DirectoryInfo(BackupersPath);

            if(!directory.Exists) {
                directory.Create();
                return backupers;
            }

            var files = directory.GetFiles();

            foreach(FileInfo file in files) {
                backupers.Add(Load(file.FullName));
            }

            return backupers;
        }

        private static Backuper Load(string path) {
            string[] lines = File.ReadAllLines(path);
            return Backuper.Parse(lines);
        }


    }
}
