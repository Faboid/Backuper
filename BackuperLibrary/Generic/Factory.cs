using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackuperLibrary.UISpeaker;

namespace BackuperLibrary.Generic {
    public static class Factory {

        public static BackuperResultInfo CreateBackupResult(string nameBackup, BackuperResult result, Exception ex = null) {
            return new BackuperResultInfo(nameBackup, result, ex);
        }

    }
}
