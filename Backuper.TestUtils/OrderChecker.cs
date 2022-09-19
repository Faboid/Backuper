namespace Backuper.TestUtils;

/// <summary>
/// Used to ensure the operations are executed in a specific order.
/// </summary>
public class OrderChecker : IDisposable {

    public int CurrentOperation { get; private set; } = 0;
    private readonly object lockObj = new();
    private bool isDisposed = false;

    /// <summary>
    /// Signals that the operation number <paramref name="operationNum"/> has been completed.
    /// <br/>This method will make sure that the given <paramref name="operationNum"/> is the one following the previous operation number.
    /// If it's not, this method will throw <see cref="OrderException"/>.
    /// </summary>
    /// <param name="operationNum"></param>
    /// <exception cref="OrderException"></exception>
    public void Done(int operationNum) {
        lock(lockObj) {
            CurrentOperation++;
            if(CurrentOperation != operationNum) {
                throw new OrderException($"The operation number {operationNum} has occurred instead of the number {CurrentOperation}");
            }
        }
    }

    /// <summary>
    /// Uses <see cref="Task.Delay(int)"/> to wait asynchronously until <see cref="CurrentOperation"/> is equal or above of <paramref name="operationToWaitFor"/>.
    /// <br/><br/>
    /// If it takes more milliseconds than <paramref name="millisecondsTimeout"/>, it throws a <see cref="TimeoutException"/>.
    /// </summary>
    /// <param name="operationToWaitFor"></param>
    /// <param name="millisecondsTimeout"></param>
    /// <returns></returns>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public async Task WaitForAsync(int operationToWaitFor, int millisecondsTimeout) {

        int awaited = 0;
        int cycle = 10;
        while(CurrentOperation < operationToWaitFor) {
            if(awaited > millisecondsTimeout) {
                throw new TimeoutException($"The operation number {operationToWaitFor} hasn't been reached. Current operation: {CurrentOperation}");
            }
            if(isDisposed) {
                throw new ObjectDisposedException(nameof(OrderChecker));
            }
            await Task.Delay(cycle);
            awaited += cycle;
        }

    }

    /// <summary>
    /// Uses <see cref="Thread.Sleep(int)"/> to wait until <see cref="CurrentOperation"/> is equal or above of <paramref name="operationToWaitFor"/>.
    /// </summary>
    /// <param name="operationToWaitFor"></param>
    /// <param name="millisecondsTimeout"></param>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void WaitFor(int operationToWaitFor, int millisecondsTimeout) {
        int awaited = 0;
        int cycle = 10;
        while(CurrentOperation < operationToWaitFor) {
            if(awaited > millisecondsTimeout) {
                throw new TimeoutException($"The operation number {operationToWaitFor} hasn't been reached. Current operation: {CurrentOperation}");
            }
            if(isDisposed) {
                throw new ObjectDisposedException(nameof(OrderChecker));
            }
            Thread.Sleep(cycle);
            awaited += cycle;
        }
    }

    /// <summary>
    /// This method represents a point in code that shouldn't be reached: throws <see cref="NoRunException"/> and does nothing else.
    /// </summary>
    /// <exception cref="NoRunException"></exception>
    public static void Never() {
        throw new NoRunException("This point in code shouldn't have been reached.");
    }

    /// <summary>
    /// This method represents a point in code that shouldn't be reached: throws <see cref="NoRunException"/> with a custom <paramref name="message"/> and does nothing else.
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="NoRunException"></exception>
    public static void Never(string message) {
        throw new NoRunException(message);
    }

    public void Dispose() {
        isDisposed = true;
    }
}

internal class OrderException : Exception {

    public OrderException(string message) : base(message) {

    }

}

internal class NoRunException : Exception {

    public NoRunException(string message) : base(message) {

    }

}

