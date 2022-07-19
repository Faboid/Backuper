namespace Backuper.Extensions; 

public static class EnumerableExtensions {

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
        foreach(var item in enumerable) {
            action.Invoke(item);
        }

        return enumerable;
    }

    public static async Task<IEnumerable<T>> ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action) {
        foreach(var item in enumerable) {
            await action.Invoke(item);
        }

        return enumerable;
    }

    public static async IAsyncEnumerable<Y> SelectAsync<T, Y>(this IEnumerable<T> enumerable, Func<T, Task<Y>> func) {
        foreach(var item in enumerable) {
            yield return await func.Invoke(item);
        }
    }

    public static async IAsyncEnumerable<Y> Select<T, Y>(this IAsyncEnumerable<T> enumerable, Func<T, Y> func) {
        await foreach(var item in enumerable) {
            yield return func.Invoke(item);
        }
    }

}
