using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BackuperUI {
    public static class WindowCommands {
        
        public static void Minimize(this Window sender) {
            sender.WindowState = WindowState.Minimized;
        }

        public static void Maximize(this Window sender) {

            if(sender.WindowState == WindowState.Normal) {

                sender.WindowState = WindowState.Maximized;

            } else if(sender.WindowState == WindowState.Maximized) {

                sender.WindowState = WindowState.Normal;
            }
        }
    }
}
