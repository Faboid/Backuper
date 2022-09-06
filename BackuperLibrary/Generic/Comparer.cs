using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BackuperLibrary.IO;

namespace BackuperLibrary.Generic {
    public static class Comparer {

        /// <summary>
        /// Searches the directory's folders in search of the newest folder, then returns that folder's creation date
        /// </summary>
        /// <param name="path">The path to the directory</param>
        /// <returns>The date of the creation of the newest folder within the directory</returns>
        public static string GetLatestVersion(DirectoryInfo path) {
            //iterates through all directories(versions) and get the most recent one

            var versions = path.GetDirectories();

            if(versions.Length == 0) { return null; }

            return versions.OrderBy(x => x.CreationTime).First().ToString();
        }

    }
}
