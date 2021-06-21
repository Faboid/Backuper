using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackuperLibrary.Generic {
    public static class Settings {

        public static void SetCurrentThreadToEnglish() {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        }

        public static void IFDEBUGSetCurrentThreadToEnglish() {
#if DEBUG
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
#endif
        }

        /// <summary>
        /// Sets current thread to <see cref="Thread.IsBackground"/> = <see langword="true"/> to execute the function, then puts it back to its previous setting.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public static void SetThreadForegroundHere(Action action) {
            bool isBackgroundThread = Thread.CurrentThread.IsBackground;
            Thread.CurrentThread.IsBackground = false;

            try {
                action.Invoke();

            } finally {
                if(isBackgroundThread) {
                    Thread.CurrentThread.IsBackground = true;
                }
            }
        }

        /// <summary>
        /// Sets current thread to <see cref="Thread.IsBackground"/> = <see langword="true"/> to execute the function, then puts it back to its previous setting and returns the result.
        /// </summary>
        /// <typeparam name="T">The return of the function.</typeparam>
        /// <param name="function">The function to execute.</param>
        /// <returns>What's returned by the function.</returns>
        public static T SetThreadForegroundHere<T>(Func<T> function) {
            bool isBackgroundThread = Thread.CurrentThread.IsBackground;
            Thread.CurrentThread.IsBackground = false;

            try {
                T result = function.Invoke();
                return result;
            } finally {
                if(isBackgroundThread) {
                    Thread.CurrentThread.IsBackground = true;
                }
            }
        }

    }
}
