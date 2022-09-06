namespace Backuper.Utils;

/// <summary>
/// Provides methods to interact with <see cref="Thread.CurrentThread"/> more easily.
/// </summary>
public static class ThreadsHandler {

    /// <summary>
    /// Sets <see cref="Thread.CurrentThread"/> to foreground. <br/>
    /// Once the returned <see cref="ThreadHandler"/> is disposed, 
    /// the thread will get set back to the previous value.
    /// </summary>
    /// <returns></returns>
    public static ThreadHandler SetScopedForeground() {
        var curr = Thread.CurrentThread;
        return new(curr);
    }

    /// <summary>
    /// Provides an <see cref="IDisposable"/> implementation that sets the <see cref="Thread"/> to its previous value.
    /// </summary>
    public struct ThreadHandler : IDisposable {

        internal ThreadHandler(Thread thread) {
            wasBackground = thread.IsBackground;
            thread.IsBackground = false;
            this.thread = thread;
        }

        private readonly Thread thread;
        private readonly bool wasBackground;
        private bool isDisposed = false;

        public void Dispose() {
            if(!isDisposed) {
                thread.IsBackground = wasBackground;
                isDisposed = true;
            }
        }
    }

}
