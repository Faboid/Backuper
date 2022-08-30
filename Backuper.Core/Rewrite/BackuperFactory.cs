using Backuper.Core.Models;
using Backuper.Core.Services;
using Backuper.Core.Validation;
using Backuper.Core.Versioning;
using Backuper.Extensions;
using Backuper.Utils;

namespace Backuper.Core.Rewrite;

public class BackuperFactory : IBackuperFactory {

    private readonly IBackuperVersioningFactory _versioningFactory;
    private readonly IBackuperServiceFactory _serviceFactory;
    private readonly IBackuperConnection _connection;
    private readonly IBackuperValidator _validator;

    public BackuperFactory(IBackuperVersioningFactory versioningFactory, IBackuperServiceFactory serviceFactory, IBackuperConnection connection, IBackuperValidator validator) {
        _versioningFactory = versioningFactory;
        _serviceFactory = serviceFactory;
        _connection = connection;
        _validator = validator;
    }

    public async Task<Option<IBackuper, CreateBackuperFailureCode>> CreateBackuper(BackuperInfo info) {

        var isValid = _validator.IsValid(info);
        if(isValid != BackuperValid.Valid) {

            return isValid switch {
                BackuperValid.NameIsEmpty => CreateBackuperFailureCode.NameIsEmpty,
                BackuperValid.NameHasIllegalCharacters => CreateBackuperFailureCode.NameHasIllegalCharacters,
                BackuperValid.SourceIsEmpty => CreateBackuperFailureCode.SourceIsEmpty,
                BackuperValid.SourceDoesNotExist => CreateBackuperFailureCode.SourceDoesNotExist,
                BackuperValid.ZeroOrNegativeMaxVersions => CreateBackuperFailureCode.ZeroOrNegativeMaxVersions,
                _ => CreateBackuperFailureCode.Unknown,
            };
        }

        if(_connection.Exists(info.Name)) {
            return CreateBackuperFailureCode.NameIsOccupied;
        }

        //create backuper in db
        await _connection.SaveAsync(info);

        var service = _serviceFactory.CreateBackuperService(info.SourcePath);
        var versioning = _versioningFactory.CreateVersioning(info.Name);
        Backuper backuper = new(info, service, _connection, versioning, _validator);
        return backuper;

    }

    public IAsyncEnumerable<IBackuper> LoadBackupers() {
        return _connection
            .GetAllBackupersAsync()
            //todo - consider what to do when the source paths don't exist anymore
            .Select(x => (info: x, versioning: _versioningFactory.CreateVersioning(x.Name), service: _serviceFactory.CreateBackuperService(x.SourcePath)))
            .Select(x => new Backuper(x.info, x.service, _connection, x.versioning, _validator));
    }

    public enum CreateBackuperFailureCode {
        Unknown,
        ZeroOrNegativeMaxVersions,
        SourceDoesNotExist,
        SourceIsEmpty,
        NameHasIllegalCharacters,
        NameIsEmpty,
        NameIsOccupied
    }

}
