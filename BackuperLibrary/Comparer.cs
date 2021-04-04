using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BackuperLibrary {
    public static class Comparer {

        public static bool AreDifferent(string firstPath, string secondPath) {

            string[] firstFilesPaths = Directory.GetFiles(firstPath);
            string[] secondFilesPaths = Directory.GetFiles(secondPath);
            if(firstFilesPaths != secondFilesPaths) { return false; }

            string[] firstFilesContent = Reader.GetFilesContent(firstFilesPaths);
            string[] secondFilesContent = Reader.GetFilesContent(secondFilesPaths);
            if(firstFilesContent != secondFilesContent) { return false; }

            return true;
        }

        public static string GetLatestVersion(string path) {
            //iterates through all directories(versions) and get the most recent one

            string[] versions = Directory.GetDirectories(path);
            
            if(versions.Length == 0) { return null; }

            return versions.OrderBy(x => Directory.GetCreationTime(x)).First();
        }

    }
}
