using BackuperLibrary;
using BackuperLibrary.UISpeaker;
using BackuperUI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BackuperUI.UIClasses {
    public static class Backuping {

        /// <summary>
        /// Shows error message to the user.
        /// </summary>
        /// <param name="ex">The exception that contains the message to show.</param>
        /// <param name="dispatcher">The dispatcher to stay on the UI thread.</param>
        public static void ShowError(Exception ex, Dispatcher dispatcher) {
            DarkMessageBox.Show("Something went wrong!", ex.Message, dispatcher);
        }

        /// <summary>
        /// Shows the results of multiple backups to the user.
        /// </summary>
        /// <param name="results">The results to show.</param>
        /// <param name="dispatcher">The dispatcher to stay on the UI thread.</param>
        public static void ShowResultsToUser(IEnumerable<BackuperResultInfo> results, Dispatcher dispatcher) {

            if(results is null) {
                DarkMessageBox.Show("Something went wrong!", "The list of the results is null.", dispatcher);
            }

            ResultsHandler.GetResults(results, out int succeeded, out int updated, out int errors);

            if(errors == 0) {
                DarkMessageBox.Show("Backup Complete!",
                    $"{succeeded} have been backuped successfully.\r\n" +
                    $"{updated} were already updated.\r\n" +
                    $"{errors} met failure."
                    , dispatcher);
            } else {
                var userAnswer = DarkMessageBox.Show("Backup Complete!",
                    $"{succeeded} have been backuped successfully.\r\n" +
                    $"{updated} were already updated.\r\n" +
                    $"{errors} met failure.\r\n \r\n" +
                    "Do you want to see the error messages?"
                    , dispatcher
                    , MessageBoxButton.YesNo);

                if(userAnswer == MessageBoxResult.No) {
                    return;
                }

                var failures = results.Where(x => x.Result == BackuperResult.Failure);

                int totalcount = failures.Count();
                int currentcount = 0;
                string chooseMessage = $"{Environment.NewLine}{Environment.NewLine}Do you want to read the next error?";
                foreach(BackuperResultInfo failure in failures) {
                    currentcount++;

                    var answer = DarkMessageBox.Show("Error:", $"{failure.GetMessage()}{((currentcount == totalcount) ? "" : chooseMessage)}", dispatcher, MessageBoxButton.YesNo);
                    if(answer == MessageBoxResult.No) {
                        break;
                    }
                }
            }

        }

    }
}
