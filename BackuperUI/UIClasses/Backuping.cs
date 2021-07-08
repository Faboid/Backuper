using BackuperLibrary;
using BackuperLibrary.UISpeaker;
using BackuperUI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BackuperUI.UIClasses {
    public static class Backuping {

        public static void ShowError(Exception ex) {
            DarkMessageBox.Show("Something went wrong!", ex.Message);
        }

        public static void ShowResultsToUser(IEnumerable<BackuperResultInfo> results) {
            if(results is null) {
                DarkMessageBox.Show("Something went wrong!", "The list of the results is null.");
            }

            ResultsHandler.GetResults(results, out int succeeded, out int updated, out int errors);

            if(errors == 0) {
                DarkMessageBox.Show("Backup Complete!",
                    $"{succeeded} have been backuped successfully.\r\n" +
                    $"{updated} were already updated.\r\n" +
                    $"{errors} met failure."
                    );
            } else {
                var userAnswer = DarkMessageBox.Show("Backup Complete!",
                    $"{succeeded} have been backuped successfully.\r\n" +
                    $"{updated} were already updated.\r\n" +
                    $"{errors} met failure.\r\n \r\n" +
                    "Do you want to see the error messages?"
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

                    var answer = DarkMessageBox.Show("Error:", $"{failure.GetMessage()}{((currentcount == totalcount) ? "" : chooseMessage)}", MessageBoxButton.YesNo);
                    if(answer == MessageBoxResult.No) {
                        break;
                    }
                }
            }
        }

    }
}
