namespace Backuper.Utils; 

public static class ThreadsHandler {

    public static ThreadHandler SetScopedForeground() {
        var curr = Thread.CurrentThread.IsBackground;
        Thread.CurrentThread.IsBackground = false;
        return new(curr);
    }

    public struct ThreadHandler : IDisposable {

        internal ThreadHandler(bool previousValue) { 
            this.previousValue = previousValue;
        }

        private readonly bool previousValue;
        private bool isDisposed = false;
        
        public void Dispose() {
            if(!isDisposed) {
                Thread.CurrentThread.IsBackground = previousValue;
                isDisposed = true;
            }
        }
    }

}
