using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackuperLibrary.Safety {

    /// <summary>
    /// Class to function as a lock.
    /// </summary>
    public class Locker {

        /// <summary>
        /// Initializes a Locker.
        /// </summary>
        /// <param name="messageErrorIfOccupied">The exception's message that will be returned in case the locker throws a <see cref="TimeoutException"/></param>
        public Locker(string messageErrorIfOccupied) {
            MessageErrorIfOccupied = messageErrorIfOccupied;
        }

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private string MessageErrorIfOccupied;


        /// <summary>
        /// Checks whether the lock is free.
        /// </summary>
        /// <returns><see langword="true"/> if the lock is open; otherwise, <see langword="false"/>.</returns>
        public bool CheckLock() {
            return semaphore.CurrentCount > 0;
        }

        /// <summary>
        /// Locks within the function, then unlocks. Throws a <see cref="TimeoutException"/> if it can't lock within 10ms.
        /// </summary>
        /// <param name="action">The action to execute while locked.</param>
        /// <exception cref="TimeoutException"></exception>
        public void LockHere(Action action) {
            Lock();
            try {
                action.Invoke();
            } finally {
                Unlock();
            }
        }

        /// <summary>
        /// Locks within the function, unlocks, and then returns a value of generic type. Throws a <see cref="TimeoutException"/> if it can't lock within 10ms.
        /// </summary>
        /// <typeparam name="T">The type of the value to return.</typeparam>
        /// <param name="action">The action to execute while locked.</param>
        /// <returns><paramref name="action"/>'s return value.</returns>
        /// <exception cref="TimeoutException"></exception>
        public T LockHere<T>(Func<T> action) {
            Lock();
            try {
                return action.Invoke();
            } finally {
                Unlock();
            }
        }

        /// <summary>
        /// Attempts locking for 10ms. If it can't, it throws a <see cref="TimeoutException"/>.
        /// </summary>
        /// <exception cref="TimeoutException"></exception>
        private void Lock() {
            if(!semaphore.Wait(10)) {
                throw new TimeoutException(MessageErrorIfOccupied);
            }
        }

        /// <summary>
        /// Unlocks the lock.
        /// </summary>
        private void Unlock() {
            semaphore.Release();
        }


    }
}
