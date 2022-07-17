namespace Backuper.Utils;

/// <summary>
/// A wrapper around <see cref="SemaphoreSlim"/> to give syntax sugar making use of <see langword="using"/> statements and <see cref="IDisposable"/>. 
/// Each entry will gain a <see cref="Lock"/> that, on disposal, will release the <see cref="SemaphoreSlim"/>.
/// </summary>
public class Locker : IDisposable {

    /// <summary>
    /// Initializes a <see cref="Locker"/> that allows a single concurrent entry.
    /// </summary>
    public Locker() : this(1) { }

    /// <summary>
    /// Initializes a <see cref="Locker"/> with a custom amount of concurrent entries.
    /// </summary>
    /// <param name="concurrentEntry"></param>
    public Locker(int concurrentEntry) {
        semaphore = new(concurrentEntry, concurrentEntry);
    }

    private readonly SemaphoreSlim semaphore;
    private bool isDisposed = false;

    /// <summary>
    /// If <see cref="Locker"/> is disposed, it will throw an <see cref="ObjectDisposedException"/>.
    /// </summary>
    /// <exception cref="ObjectDisposedException"></exception>
    private void ThrowIfDisposed() {
        if(isDisposed) {
            throw new ObjectDisposedException(nameof(Locker));
        }
    }

    /// <summary>
    /// Blocks the current thread until it can enter.
    /// </summary>
    /// <returns>A <see cref="Lock"/> that can be disposed to release the resources once more.</returns>
    /// <exception cref="ObjectDisposedException"></exception>
    public Lock GetLock() {
        semaphore.Wait();
        ThrowIfDisposed();
        return new(true, this);
    }

    /// <summary>
    /// Blocks the current thread until it can enter, using <paramref name="millisecondsTimeout"/> milliseconds as timeout.
    /// </summary>
    /// <param name="millisecondsTimeout">The maximum milliseconds that will be waited to enter.</param>
    /// <returns>A <see cref="Lock"/> that can be disposed to release the resources once more. 
    /// Check <see cref="Lock.Obtained"/> to know if the resources have been obtained successfully.</returns>
    /// <exception cref="ObjectDisposedException"></exception>
    public Lock GetLock(int millisecondsTimeout) {
        bool result = semaphore.Wait(millisecondsTimeout);

        ThrowIfDisposed();
        return new(result, this);
    }

    /// <summary>
    /// Blocks the current thread until it can enter, using <paramref name="millisecondsTimeout"/> milliseconds as timeout, while observing <paramref name="cancellationToken"/>.
    /// </summary>
    /// <param name="millisecondsTimeout">The maximum milliseconds that will be waited to enter.</param>
    /// <param name="cancellationToken">The token that will be observed.</param>
    /// <returns>A <see cref="Lock"/> that can be disposed to release the resources once more. 
    /// Check <see cref="Lock.Obtained"/> to know if the resources have been obtained successfully.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public Lock GetLock(int millisecondsTimeout, CancellationToken cancellationToken) {
        bool result = semaphore.Wait(millisecondsTimeout, cancellationToken);

        ThrowIfDisposed();
        return new(result, this);
    }

    /// <summary>
    /// Waits asynchronously until it can enter.
    /// </summary>
    /// <returns>A <see cref="Lock"/> that can be disposed to release the resources once more.</returns>
    /// <exception cref="ObjectDisposedException"></exception>
    public async Task<Lock> GetLockAsync() {
        await semaphore.WaitAsync().ConfigureAwait(false);
        ThrowIfDisposed();
        return new(true, this);
    }

    /// <summary>
    /// Waits asynchronously until it can enter, using <paramref name="millisecondsTimeout"/> milliseconds as timeout.
    /// </summary>
    /// <param name="millisecondsTimeout">The maximum milliseconds that will be waited to enter.</param>
    /// <returns>A <see cref="Lock"/> that can be disposed to release the resources once more. 
    /// Check <see cref="Lock.Obtained"/> to know if the resources have been obtained successfully.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public async Task<Lock> GetLockAsync(int millisecondsTimeout) {
        bool result = await semaphore.WaitAsync(millisecondsTimeout).ConfigureAwait(false);
        ThrowIfDisposed();
        return new(result, this);
    }

    /// <summary>
    /// Waits asynchronously until it can enter while observing <paramref name="cancellationToken"/>.
    /// </summary>
    /// <param name="millisecondsTimeout">The maximum milliseconds that will be waited to enter.</param>
    /// <param name="cancellationToken">The token that will be observed.</param>
    /// <returns>A <see cref="Lock"/> that can be disposed to release the resources once more. 
    /// Check <see cref="Lock.Obtained"/> to know if the resources have been obtained successfully.</returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<Lock> GetLockAsync(CancellationToken cancellationToken) {
        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        ThrowIfDisposed();
        return new(true, this);
    }

    /// <summary>
    /// Waits asynchronously until it can enter, using <paramref name="millisecondsTimeout"/> milliseconds as timeout, while observing <paramref name="cancellationToken"/>.
    /// </summary>
    /// <param name="millisecondsTimeout">The maximum milliseconds that will be waited to enter.</param>
    /// <param name="cancellationToken">The token that will be observed.</param>
    /// <returns>A <see cref="Lock"/> that can be disposed to release the resources once more. 
    /// Check <see cref="Lock.Obtained"/> to know if the resources have been obtained successfully.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public async Task<Lock> GetLockAsync(int millisecondsTimeout, CancellationToken cancellationToken) {
        bool result = await semaphore.WaitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(false);
        ThrowIfDisposed();
        return new(result, this);
    }

    private void Unlock() {

        //even if it's disposed, Unlock() should be a valid call
        if(isDisposed) {
            return;
        }

        semaphore.Release();
    }

    private void ReleaseAll() {
        while(semaphore.CurrentCount < 1) {
            semaphore.Release();
        }
    }

    public void Dispose() {
        lock(semaphore) {
            if(isDisposed) {
                return;
            }

            isDisposed = true;
            ReleaseAll();
            semaphore.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public struct Lock : IDisposable {

        private readonly Locker locker;
        private bool isDisposed = false;

        public readonly bool Obtained { get; init; }

        internal Lock(bool obtained, Locker locker) {
            this.locker = locker;
            Obtained = obtained;
        }

        public void Dispose() {
            if(isDisposed) {
                throw new ObjectDisposedException(nameof(Lock));
            }

            isDisposed = true;
            if(Obtained) {
                locker.Unlock();
            }
        }
    }

}
