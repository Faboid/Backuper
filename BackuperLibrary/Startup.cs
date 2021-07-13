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

    /// <summary>
    /// Deals with turning ON/OFF the possibility of this app to turn on at the window user's start up.
    /// </summary>
    public static class Startup {

        private static readonly string pathToExe = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        private static readonly string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        private static readonly string shortcutPath = Path.Combine(startupPath, "Backuper.lnk");

        /// <summary>
        /// The argument given by the startup shortcut.
        /// </summary>
        public const string startupArgument = "Startup";

        /// <summary>
        /// Checks if the shortcut exists.
        /// </summary>
        /// <returns><see langword="True"/> if the shortcut has been found; otherwise, <see langword="False"/></returns>
        public static bool IsActive() {
            return System.IO.File.Exists(shortcutPath);
        }

        /// <summary>
        /// If <paramref name="set"/> is true, creates the shortcut to turn on the app; if it's false, it deletes it.
        /// </summary>
        /// <param name="set">Whether to create(true) or delete(false) the shortcut to allow the turning on of the app at the window user's startup.</param>
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
