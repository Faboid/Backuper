﻿using Backuper.Utils;
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

    public static Option<BackuperInfo> TryParse(string[] values) {

        if(values.Length != 4) {
            return Option.None<BackuperInfo>();
        }

        var name = values[0];
        var source = values[1];

        if(!int.TryParse(values[2], out var maxVersions) || maxVersions < 1) {
            return Option.None<BackuperInfo>();
        }

        if(!bool.TryParse(values[3], out var updateOnBoot)) { 
            return Option.None<BackuperInfo>();
        }

        return new BackuperInfo(name, source, maxVersions, updateOnBoot);

    }

}
