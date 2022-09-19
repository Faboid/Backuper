using Backuper.Core.Models;
using Backuper.Core.Saves;
using Backuper.Core.Services;
using Backuper.Core.Validation;
using Backuper.Core.Versioning;
using Backuper.Extensions;
using Backuper.Utils;
using Microsoft.Extensions.Logging;

namespace Backuper.Core;

public class BackuperFactory : IBackuperFactory {

    private readonly ILogger<IBackuperFactory> _logger;
    private readonly ILogger<IBackuper> _backuperLogger;
    private readonly IBackuperVersioningFactory _versioningFactory;
    private readonly IBackuperServiceFactory _serviceFactory;
    private readonly IBackuperConnection _connection;
    private readonly IBackuperValidator _validator;

    public BackuperFactory(IBackuperVersioningFactory versioningFactory, 
                            IBackuperServiceFactory serviceFactory, 
                            IBackuperConnection connection, 
                            IBackuperValidator validator, 
                            ILogger<IBackuperFactory> logger, 
                            ILogger<IBackuper> backuperLogger) {
        _versioningFactory = versioningFactory;
        _serviceFactory = serviceFactory;
        _connection = connection;
        _validator = validator;
        _logger = logger;
        _backuperLogger = backuperLogger;
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
        _logger.LogInformation("A new backuper, {Name}, has been created.", info.Name);

        var service = _serviceFactory.CreateBackuperService(info.SourcePath);
        var versioning = _versioningFactory.CreateVersioning(info.Name);
        Backuper backuper = new(info, service, _connection, versioning, _validator, _backuperLogger);
        return backuper;

    }

    public async IAsyncEnumerable<IBackuper> LoadBackupers() {

        await foreach(var optionInfo in _connection.GetAllBackupersAsync()) {

            yield return optionInfo.Match(
                some => {
                    var versioning = _versioningFactory.CreateVersioning(some.Name);
                    var backupingService = _serviceFactory.CreateBackuperService(some.SourcePath);
                    return new Backuper(some, backupingService, _connection, versioning, _validator, _backuperLogger);
                },

                name => {
                    var versioning = _versioningFactory.CreateVersioning(name);
                    var backupingService = _serviceFactory.CreateCorruptedService();
                    return new Backuper(new BackuperInfo(name, "Unknown", 3, false), backupingService, _connection, versioning, _validator, _backuperLogger);
                },

                () => {
                    //as it's needed to have the name to create a backuper, this is impossible to resolve
                    _logger.LogError("LoadBackupers tried to resolve an Option.None");
                    throw new InvalidOperationException("LoadBackupers tried to resolve an Option.None");
                });
        }
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
