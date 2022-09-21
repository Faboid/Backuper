namespace Backuper.Extensions;

public static class FileInfoExtensions {

    /// <summary>
    /// Copies an existing file to a new file asynchronously.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async Task CopyToAsync(this FileInfo info, string path) {
        using var reader = info.OpenRead();
        using var writer = File.Create(path);
        await reader.CopyToAsync(writer);
    }

}
