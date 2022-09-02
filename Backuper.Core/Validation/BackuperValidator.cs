using Backuper.Abstractions;
using Backuper.Core.Models;
namespace Backuper.Core.Validation; 

public class BackuperValidator : IBackuperValidator {

    private readonly IDirectoryInfoProvider _directoryInfoProvider;
    private readonly IFileInfoProvider _fileInfoProvider;

    public BackuperValidator(IDirectoryInfoProvider directoryInfoProvider, IFileInfoProvider fileInfoProvider) {
        _directoryInfoProvider = directoryInfoProvider;
        _fileInfoProvider = fileInfoProvider;
    }

    public NameValid IsNameValid(string name) {
        
        if(string.IsNullOrWhiteSpace(name)) {
            return NameValid.EmptyOrWhiteSpace;
        }

        if(Path.GetInvalidFileNameChars().Any(x => name.Contains(x))) {
            return NameValid.HasIllegalCharacters;
        }

        return NameValid.Valid;

    }

    public SourcePathValid IsSourcePathValid(string sourcePath) {
        
        if(string.IsNullOrWhiteSpace(sourcePath)) {
            return SourcePathValid.EmptyOrWhiteSpace;
        }

        if(!_directoryInfoProvider.FromDirectoryPath(sourcePath).Exists && !_fileInfoProvider.FromFilePath(sourcePath).Exists) {
            return SourcePathValid.DoesNotExist;
        }

        return SourcePathValid.Valid;
    }

    public MaxVersionsValid IsMaxVersionsValid(int maxVersions) {
        
        if(maxVersions < 1) {
            return MaxVersionsValid.LessThanOne;
        }

        return MaxVersionsValid.Valid;
    }

    public BackuperValid IsValid(BackuperInfo info) {

        if(info == null) {
            return BackuperValid.IsNull;
        }

        var nameValid = IsNameValid(info.Name);
        if(nameValid != NameValid.Valid) {

            return nameValid switch {
                NameValid.EmptyOrWhiteSpace => BackuperValid.NameIsEmpty,
                NameValid.HasIllegalCharacters => BackuperValid.NameHasIllegalCharacters,
                _ => BackuperValid.Unknown,
            };
        }

        var sourceValid = IsSourcePathValid(info.SourcePath);
        if(sourceValid != SourcePathValid.Valid) {

            return sourceValid switch {
                SourcePathValid.EmptyOrWhiteSpace => BackuperValid.SourceIsEmpty,
                SourcePathValid.DoesNotExist => BackuperValid.SourceDoesNotExist,
                _ => BackuperValid.Unknown,
            };

        }

        var maxVersionsValid = IsMaxVersionsValid(info.MaxVersions);
        if(maxVersionsValid != MaxVersionsValid.Valid) {

            return maxVersionsValid switch {
                MaxVersionsValid.LessThanOne => BackuperValid.ZeroOrNegativeMaxVersions,
                _ => BackuperValid.Unknown,
            };

        }

        return BackuperValid.Valid;
    }
}
