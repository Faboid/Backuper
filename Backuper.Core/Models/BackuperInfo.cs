using System.Text;

namespace Backuper.Core.Models; 

public class BackuperInfo {

    public BackuperInfo(string name, string sourcePath, int maxVersions, bool updateOnBoot) {

        if(maxVersions < 1) {
            throw new ArgumentOutOfRangeException(nameof(maxVersions), "The maximum versions cannot be less than one.");
        }

        Name = name;
        SourcePath = sourcePath;
        MaxVersions = maxVersions;
        UpdateOnBoot = updateOnBoot;
    }

    /// <summary>
    /// The key of this backuper. There cannot be multiple backupers with the same name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The path to the source; to what this backuper backups.
    /// </summary>
    public string SourcePath { get; set; }

    /// <summary>
    /// The maximum versions allowed for this backuper.
    /// </summary>
    public int MaxVersions { get; set; }

    /// <summary>
    /// Whether this backuper gets backuped on computer boot.
    /// </summary>
    public bool UpdateOnBoot { get; set; }

    public InfoValid IsValid() {

        if(string.IsNullOrWhiteSpace(Name)) {
            return InfoValid.NameEmptyOrSpaces;
        }

        if(!Directory.Exists(SourcePath) && !File.Exists(SourcePath)) {
            return InfoValid.SourceDoesNotExist;
        }

        if(MaxVersions < 1) {
            return InfoValid.NegativeMaxVersions;
        }

        return InfoValid.Valid;
    }

    public enum InfoValid {
        Unknown,
        Valid,
        NameEmptyOrSpaces,
        SourceDoesNotExist,
        NegativeMaxVersions,
    }

    private const string separator = ",";
    public override string ToString() {
        var values = ToStrings();
        return new StringBuilder()
            .AppendJoin(separator, values)
            .ToString();
    }

    public string[] ToStrings() {
        return new string[] { Name, SourcePath, MaxVersions.ToString(), UpdateOnBoot.ToString() };
    }

    public static BackuperInfo Parse(string s) {
        var values = s.Split(separator, StringSplitOptions.None);
        return Parse(values);
    }

    public static BackuperInfo Parse(string[] values) {
        return new(values[0], values[1], int.Parse(values[2]), bool.Parse(values[3]));
    }

}
