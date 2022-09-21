using System.Diagnostics;

namespace Backuper.Utils;

/// <summary>
/// Provides methods to interact with windows explorer.
/// </summary>
public class Explorer {

    /// <summary>
    /// Attempts to open a folder through windows explorer.
    /// </summary>
    /// <param name="path"></param>
    /// <returns>Whether it was successful.</returns>
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