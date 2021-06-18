using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackuperLibrary.Safety {
    public class Locker {

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);


        /// <summary>
        /// Checks whether the lock is free.
        /// </summary>
        /// <returns><see langword="true"/> if the lock is open; otherwise, <see langword="false"/>.</returns>
        public bool CheckLock() {
            return semaphore.CurrentCount > 0;
        }

        public void LockHere(Action action) {
            Lock();
            try {
                action.Invoke();
            } finally {
                Unlock();
            }
        }

        public T LockHere<T>(Func<T> action) {
            Lock();
            try {
                return action.Invoke();
            } finally {
                Unlock();
            }
        }

        public void Lock() {
            if(!semaphore.Wait(10)) {
                throw new ArgumentException("This backuper is being used elsewhere.");
            }
        }

        public void Unlock() {
            semaphore.Release();
        }


    }
}
