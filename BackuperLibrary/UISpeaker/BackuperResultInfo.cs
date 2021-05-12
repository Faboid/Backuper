using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackuperLibrary.UISpeaker {
    
    public class BackuperResultInfo {

        public BackuperResultInfo(string nameBackup, BackuperResult result, Exception ex = null) {
            NameBackup = nameBackup;
            Result = result;
            Error = ex;
        }

        public string NameBackup { get; private set; }
        public BackuperResult Result { get; private set; }
        public Exception Error { get; private set; }

        private string successfulUpdatedMessage { get => $"{NameBackup} has been updated successfully."; }
        private string alreadyUpdatedMessage { get => $"{NameBackup} is already updated."; }
        private string errorMessage { get => $"{NameBackup} has been met by an error: {Error.Message}"; }

        public string GetMessage() => Result switch {
            BackuperResult.Success => successfulUpdatedMessage,
            BackuperResult.AlreadyUpdated => alreadyUpdatedMessage,
            BackuperResult.Failure => errorMessage
        };

    }

    public enum BackuperResult {
        Success,
        AlreadyUpdated,
        Failure
    }

}
