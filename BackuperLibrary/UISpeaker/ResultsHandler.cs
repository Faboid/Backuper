using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackuperLibrary.UISpeaker {
    public static class ResultsHandler {

        /// <summary>
        /// Converts a list of <see cref="BackuperResultInfo"/> into three ints to represent successes, failures, and already updated backupers.
        /// </summary>
        /// <param name="results">The list of backups' results.</param>
        /// <param name="successes">How many successes were in the list.</param>
        /// <param name="alreadyUpdated">How many "already updated" results in the list.</param>
        /// <param name="failures">How many failures on the list.</param>
        public static void GetResults(this IEnumerable<BackuperResultInfo> results, out int successes, out int alreadyUpdated, out int failures) {
            successes = 0;
            alreadyUpdated = 0;
            failures = 0;

            foreach(BackuperResultInfo result in results) {
                _ = result.Result switch {
                    BackuperResult.Success => successes++,
                    BackuperResult.AlreadyUpdated => alreadyUpdated++,
                    BackuperResult.Failure => failures++,
                    _ => throw new ArgumentException("A result was found with invalid values.")
                };
            }
        }

    }
}
