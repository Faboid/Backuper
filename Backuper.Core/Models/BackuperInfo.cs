using Backuper.Utils;
using System.Text;

namespace Backuper.Core.Models;

/// <summary>
/// Contains all the info required for the functioning of a <see cref="Backuper"/>.
/// </summary>
public class BackuperInfo {

    /// <summary>
    /// Instances a <see cref="BackuperInfo"/> with the given values.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="sourcePath"></param>
    /// <param name="maxVersions"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public BackuperInfo(string name, string sourcePath, int maxVersions) {

        if(maxVersions < 1) {
            throw new ArgumentOutOfRangeException(nameof(maxVersions), "The maximum versions cannot be less than one.");
        }

        Name = name;
        SourcePath = sourcePath;
        MaxVersions = maxVersions;
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

    private const string separator = ",";
    public override string ToString() {
        var values = ToStrings();
        return new StringBuilder()
            .AppendJoin(separator, values)
            .ToString();
    }

    /// <summary>
    /// Returns a <see cref="string[]"/> that represents the current object.
    /// </summary>
    /// <returns></returns>
    public string[] ToStrings() {
        return new string[] { Name, SourcePath, MaxVersions.ToString() };
    }

    /// <summary>
    /// Converts the string representation of a <see cref="BackuperInfo"/> into its instanced equivalent.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static BackuperInfo Parse(string s) {
        var values = s.Split(separator, StringSplitOptions.None);
        return Parse(values);
    }

    /// <summary>
    /// Converts the string[] representation of a <see cref="BackuperInfo"/> into its instanced equivalent.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static BackuperInfo Parse(string[] values) {
        return new(values[0], values[1], int.Parse(values[2]));
    }

    /// <summary>
    /// Tries to converts the string[] representation of a <see cref="BackuperInfo"/> into its instanced equivalent.
    /// If it fails, it returns <see cref="Option.None{TValue}"/>
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static Option<BackuperInfo> TryParse(string[] values) {

        if(values.Length != 3) {
            return Option.None<BackuperInfo>();
        }

        var name = values[0];
        var source = values[1];

        if(!int.TryParse(values[2], out var maxVersions) || maxVersions < 1) {
            return Option.None<BackuperInfo>();
        }

        return new BackuperInfo(name, source, maxVersions);

    }

}
