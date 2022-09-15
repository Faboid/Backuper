using Backuper.Abstractions;

namespace Backuper.Utils;

public class Settings {

    private readonly IFileInfo _settingsFile;

    public Settings(IFileInfo settingsFile) {
        _settingsFile = settingsFile;
    }

    private const string _separator = ":";

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