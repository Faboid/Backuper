using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BackuperLibrary.Generic {
    public static class Reader {

        public static string[] GetFilesContent(string[] paths) {
            List<string> output = new List<string>();

            foreach(string path in paths) {
                output.Add(File.ReadAllBytes(path).ToString());
            }

            return output.ToArray();
        }

    }
}
