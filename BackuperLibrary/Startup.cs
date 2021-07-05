using BackuperLibrary.Generic;
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BackuperLibrary {
    public static class Startup {

        private static readonly string pathToExe = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        private static readonly string startPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        private static readonly string shortcutPath = Path.Combine(startPath, "Backuper.lnk");

        public const string startupArgument = "Startup";

        public static bool IsActive() {
            return System.IO.File.Exists(shortcutPath);
        }

        public static void Set(bool set) {
            if(set) {
                CreateShortcut();
            } else {
                DeleteShortcut();
            }
        }

        private static void CreateShortcut() {
            IWshShortcut shortcut = new WshShell().CreateShortcut(shortcutPath);
            shortcut.TargetPath = pathToExe;
            shortcut.Arguments = startupArgument;
            shortcut.Description = "A shortcut to load Backuper on startup, so that it can back up everything automatically.";
            shortcut.Save();
        }

        private static void DeleteShortcut() {
            if(System.IO.File.Exists(shortcutPath)) {
                System.IO.File.Delete(shortcutPath);
            }
        }

    }
}
