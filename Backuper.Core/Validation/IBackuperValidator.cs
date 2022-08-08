using Backuper.Core.Models;

namespace Backuper.Core.Validation; 

public interface IBackuperValidator {

    BackuperValid IsValid(BackuperInfo info);
    NameValid IsNameValid(string name);
    SourcePathValid IsSourcePathValid(string sourcePath);
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

