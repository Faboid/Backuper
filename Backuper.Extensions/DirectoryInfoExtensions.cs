namespace Backuper.Extensions;

public static class DirectoryInfoExtensions {

    /// <summary>
    /// Copies to another directory. All subdirectories and files will be copied to it with their relative paths.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="path"></param>
    public static void CopyTo(this DirectoryInfo info, string path) {
        Directory.CreateDirectory(path);

        //create all directories
        info
            .EnumerateDirectories("*", SearchOption.AllDirectories)
            .Select(x => x.FullName.Replace(info.FullName, path))
            .ForEach(x => Directory.CreateDirectory(x));

        //copy all files
        info
            .EnumerateFiles("*", SearchOption.AllDirectories)
            .Select(x => (File: x, NewPath: x.FullName.Replace(info.FullName, path)))
            .ForEach(x => x.File.CopyTo(x.NewPath));
    }

    /// <summary>
    /// Copies to another directory asynchronously. All subdirectories and files will be copied to it with their relative paths.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async Task CopyToAsync(this DirectoryInfo info, string path) {
        Directory.CreateDirectory(path);

        //create all directories
        info
            .EnumerateDirectories("*", SearchOption.AllDirectories)
            .Select(x => x.FullName.Replace(info.FullName, path))
            .ForEach(x => Directory.CreateDirectory(x));

        //copy all files
        await info.EnumerateFiles("*", SearchOption.AllDirectories)
            .Select(x => (File: x, NewFile: x.FullName.Replace(info.FullName, path)))
            .ForEachAsync(x => x.File.CopyToAsync(x.NewFile));
    }

}
