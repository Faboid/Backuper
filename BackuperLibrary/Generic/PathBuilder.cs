using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BackuperLibrary.Generic {
    public static class PathBuilder {

        //todo - implement a way to set To's value
        public static string To { get; private set; } = @"D:\Programming\Small Projects\Backuper\TemporaryTestFolder\To"; //temporary value to test stuff

        public static string GetToPath(string name) => Path.Combine(To, name);

        public static string ChangeFormat(string input) {
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

        public static bool ValidatePath(string path, out string message) {
            message = "";
            // invalid characters: \ / : * ? " < > | 
            char[] invalidCharacters = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

            if(path.Any(x => invalidCharacters.Contains(x))) {
                message = "The name cannot contain any of the following characters: \\ / : * ? \" < > |";
                return false;
            }

            return true;
        }

    }
}
