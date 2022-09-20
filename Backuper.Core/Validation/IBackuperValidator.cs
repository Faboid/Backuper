using Backuper.Core.Models;

namespace Backuper.Core.Validation;

/// <summary>
/// Provides methods to validate a <see cref="BackuperInfo"/>.
/// </summary>
public interface IBackuperValidator {

    /// <summary>
    /// Evaluates the interity of the given <paramref name="info"/>.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    BackuperValid IsValid(BackuperInfo info);

    /// <summary>
    /// Checks if the given <see cref="BackuperInfo.Name"/> is valid.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    NameValid IsNameValid(string name);

    /// <summary>
    /// Checks if the given <see cref="BackuperInfo.SourcePath"/> is valid.
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <returns></returns>
    SourcePathValid IsSourcePathValid(string sourcePath);

    /// <summary>
    /// Checks if the given <see cref="BackuperInfo.MaxVersions"/> is valid.
    /// </summary>
    /// <param name="maxVersions"></param>
    /// <returns></returns>
    MaxVersionsValid IsMaxVersionsValid(int maxVersions);
}

public enum BackuperValid {
    Unknown,
    Valid,
    NameIsEmpty,
    NameHasIllegalCharacters,
    SourceIsEmpty,
    SourceDoesNotExist,
    ZeroOrNegativeMaxVersions,
    IsNull,
}

public enum NameValid {
    Unknown,
    Valid,
    EmptyOrWhiteSpace,
    HasIllegalCharacters
}

public enum SourcePathValid {
    Unknown,
    Valid,
    EmptyOrWhiteSpace,
    DoesNotExist,
}

public enum MaxVersionsValid {
    Unknown,
    Valid,
    LessThanOne
}

