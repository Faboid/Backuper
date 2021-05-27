﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BackuperLibrary.IO;
using BackuperLibrary.UISpeaker;

namespace BackuperLibrary {
    public static class BackupersHolder {

        public static List<Backuper> Backupers { get; private set; } = BackupersManager.LoadAll();

        static BackupersHolder() {
            BackupersManager.EditedBackupers += Refresh; ;
        }

        private static void Refresh(object sender, EventArgs e) {
            Backupers = BackupersManager.LoadAll();
        }

        public static Backuper SearchByName(string name) {
            return Backupers.Where(x => x.Name == name).Single();
        }

        public static List<BackuperResultInfo> BackupAll() {
            List<BackuperResultInfo> results = new List<BackuperResultInfo>();
            foreach(Backuper backuper in Backupers) {
                BackuperResultInfo result = backuper.MakeBackup();
                results.Add(result);
            }

            return results;
        }

    }
}