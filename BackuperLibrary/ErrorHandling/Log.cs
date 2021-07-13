using BackuperLibrary.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackuperLibrary.ErrorHandling {
    public static class Log {

        /// <summary>
        /// The path to the errors' log file.
        /// </summary>
        public static readonly string ErrorLogFile = Path.Combine(PathBuilder.GetWorkingDirectory(), "ErrorLog.txt");

        private const string separator = "///////////////////////////////";

        /// <summary>
        /// Writes an error to a log file as long as the error's type doesn't correspont to the generic T type
        /// </summary>
        /// <typeparam name="T">The generic parameter that excludes the error</typeparam>
        /// <param name="ex">The exception to write to the log file</param>
        public static void WriteErrorIfNot<T>(Exception ex) where T: Exception {
            if(ex.GetType() == typeof(T)) {
                return;
            }

            WriteError(ex);
        }

        /// <summary>
        /// Writes an error to a log file.
        /// </summary>
        /// <param name="ex">The exception to write</param>
        public static void WriteError(Exception ex) {
            File.AppendAllText(ErrorLogFile, GetMessage(ex));
        }

        /// <summary>
        /// Converts an exception into a multi-lined string value.
        /// </summary>
        /// <param name="ex">The exception to convert.</param>
        /// <returns>A multi-lined string with all the information from the exception.</returns>
        private static string GetMessage(Exception ex) {
            StringBuilder message = new StringBuilder();

            Action<int, string> AddSeparator = (int times, string separator) => {
                while(times > 0) {
                    message.AppendLine(separator);
                    times--;
                }
            };

            //divide from other errors
            AddSeparator(1, string.Empty);
            AddSeparator(2, separator);

            //add data time
            message.AppendLine(DateTime.Now.ToString());
            AddSeparator(1, string.Empty);

            //add type of exception, message, and stacktrace
            message.AppendLine(ex.ToString());
            AddSeparator(1, string.Empty);

            //add additional messages
            if(ex.Data.Count > 0) {
                message.AppendLine("Extra details:");

                foreach(DictionaryEntry de in ex.Data) {
                    AddSeparator(1, string.Empty);
                    message.AppendLine($" - [{de.Key}]: {de.Value}");
                }
            }
            
            //end of the error log
            AddSeparator(1, string.Empty);
            AddSeparator(2, separator);

            return message.ToString();
        }

    }
}
