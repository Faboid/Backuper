namespace Backuper.Utils.Tests; 

public class ThreadsHandlerTests {

    [Fact]
    public void ThreadsHandlerSetsCurrentToForeground() {

        var starting = Thread.CurrentThread.IsBackground;
        Thread.CurrentThread.IsBackground = true;

        var handler = ThreadsHandler.SetScopedForeground();
        var updated = Thread.CurrentThread.IsBackground;
        handler.Dispose();
        var afterDisposal = Thread.CurrentThread.IsBackground;

        //assert
        Assert.False(updated);
        Assert.True(afterDisposal);
        Thread.CurrentThread.IsBackground = starting;

    }

}
