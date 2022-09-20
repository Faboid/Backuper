using System.Diagnostics;

namespace Backuper.Utils;

public class Explorer {

    public static bool TryOpenFolder(string path) {

        if(!path.EndsWith(Path.DirectorySeparatorChar)) {
            path += Path.DirectorySeparatorChar;
        }

        if(!Directory.Exists(path)) {
            return false;
        }

        var info = new ProcessStartInfo() {
            FileName = path,
            UseShellExecute = true,
            Verb = "open",
        };
        
        Process.Start(info);
        return true;

    }

}