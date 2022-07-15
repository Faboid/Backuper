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

}
