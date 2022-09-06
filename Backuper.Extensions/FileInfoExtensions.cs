namespace Backuper.Extensions;

public static class FileInfoExtensions {

    public static async Task CopyToAsync(this FileInfo info, string path) {
        using var reader = info.OpenRead();
        using var writer = File.Create(path);
        await reader.CopyToAsync(writer);
    }

}
