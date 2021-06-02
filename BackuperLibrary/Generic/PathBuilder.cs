using BackuperLibrary.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BackuperLibrary.Generic {
    public static class PathBuilder {

        public static string To { get => BackupFolderHandler.To; }
        public static string BinName { get; } = "Bin";
        public static string BinBackupsFolder { get; } = Path.Combine(To, BinName);
        public static string GetWorkingDirectory() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string GetToPath(string name) => Path.Combine(To, name);
        public static string GetBinBcpsFolderPath(string name) => Path.Combine(BinBackupsFolder, $"{name}");

        public static string ChangeFormat(string input) {
            var sb = new StringBuilder(input.Length);

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

        public static bool ValidatePath(string path, out string message) {
            message = "";
            // invalid characters: \ / : * ? " < > | 
            char[] invalidCharacters = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

            if(path.Any(x => invalidCharacters.Contains(x))) {
                message = "The path cannot contain any of the following characters: \\ / : * ? \" < > |";
                return false;
            }

            return true;
        }

    }
}
