using Backuper.Abstractions;

namespace Backuper.Utils;

/// <summary>
/// Provides methods to save a key-value pair collection throughout sessions.
/// </summary>
public class Settings {

    private readonly IFileInfo _settingsFile;

    /// <summary>
    /// Initializes <see cref="Settings"/>. Will create the settings file if it doesn't exist.
    /// </summary>
    /// <param name="settingsFile"></param>
    public Settings(IFileInfo settingsFile) {
        _settingsFile = settingsFile;
        if(!_settingsFile.Exists) {
            _settingsFile.Create();
        }
    }

    private const string _separator = "%&$$!(%!";

    /// <summary>
    /// Sets <paramref name="key"/> to <paramref name="value"/>. If it exists already, it will be overwritten.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Set(string key, string value) {
        var pairs = _settingsFile
            .ReadLines()
            .Select(Pair.Split)
            .ToList();

        var line = pairs
            .FirstOrDefault(x => x?.Key == key, null);

        if(line?.Key != null) {
            line.Value = value;
        } else {
            pairs.Add(new (key, value));
        }

        _settingsFile.WriteAllLines(pairs.Select(x => x.Merge()));

    }

    /// <summary>
    /// If it exists, returns the <paramref name="key"/>'s value. If it doesn't, it returns <see cref="Option.None{TValue}"/>.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Option<string> Get(string key) {
        return _settingsFile
            .ReadLines()
            .Select(Pair.Split)
            .Where(x => x.Key == key)
            .Select<Pair, Option<string>>(x => x.Value)
            .FirstOrDefault(Option<string>.None());
    }

    private class Pair {

        public Pair(string key, string value) {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public string Value { get; set; }

        public static Pair Split(string text) {
            var arr = text.Split(_separator).Select(x => x.Trim()).ToArray();
            return new(arr[0], arr[1]);
        }

        public string Merge() {
            return $"{Key}{_separator}{Value}";
        }

    }

}