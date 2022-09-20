using Backuper.Abstractions;
using Backuper.Abstractions.TestingHelpers;
using Backuper.Core.Models;
using Backuper.Core.Saves;
using Backuper.Core.Saves.DBConnections;
using Backuper.Core.Services;
using Backuper.Core.Validation;
using Backuper.Core.Versioning;
using Backuper.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;

namespace Backuper.Core.IntegrationTests.BackuperTests;

public class BackuperTests {

	public BackuperTests() {
		_host = Host.CreateDefaultBuilder().ConfigureServices(services => {

			services.AddSingleton<IMockFileSystem, MockFileSystem>();
			services.AddSingleton<IDirectoryInfoProvider, MockDirectoryInfoProvider>();
			services.AddSingleton<IFileInfoProvider, MockFileInfoProvider>();

			services.AddSingleton<PathsHandler>();
			services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
			services.AddSingleton<IDBConnection, MemoryDBConnection>();

			services.AddSingleton<IPathsBuilderService, PathsBuilderService>();
			services.AddSingleton<IBackuperVersioningFactory, BackuperVersioningFactory>();
			services.AddSingleton<IBackuperServiceFactory, BackuperServiceFactory>();
			services.AddSingleton<IBackuperConnection, BackuperConnection>(s => new(new MemoryDBConnection()));
			services.AddSingleton<IBackuperValidator, BackuperValidator>();

			services.AddSingleton<IBackuperFactory, BackuperFactory>();

		}).Build();

		_host.Start();
	}

	private File[] GenerateFiles(string directory) {
		return _sourceFiles
            .Select(x => new File(Path.Combine(directory, x.Name), x.Text))
			.ToArray();
	}

	private readonly IHost _host;
	private record File(string Name, params string[] Text);

	private readonly string _sourceDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SomeDirSource");
	private readonly File[] _sourceFiles = new File[] { new("First.txt", "Header", "Body", "Footer"), new("EmptyFile.txt") };

	[Fact]
	public async Task Backuper_LifeCycleFromCreationToDeletion() {

		SetUpSource();
		var info = new BackuperInfo("NewBackuperName", _sourceDirectory, 3);
		var factory = _host.Services.GetRequiredService<IBackuperFactory>();

		var backuper = (await factory.CreateBackuper(info)).Or(null)!;
		await AssertBackuperDBValues(info);

		var backupResult = await backuper.BackupAsync();
		Assert.Equal(BackupResponseCode.Success, backupResult);
		Assert.True(backuper.IsUpdated());
		AssertSourceIsBackedUp(info.Name);

		info.Name = "SomeNewName";
		info.MaxVersions = 5;

		var editResult = await backuper.EditAsync(info);
		Assert.Equal(EditBackuperResponseCode.Success, editResult);
		await AssertBackuperDBValues(info);
		AssertSourceIsBackedUp(info.Name); //checking if the backups are migrated

		await backuper.BinAsync();
		AssertSourceIsBinned(info.Name);

	}

	private void AssertSourceIsHere(string path) {
        var fileSystem = _host.Services.GetRequiredService<IMockFileSystem>();
        
		GenerateFiles(path)
			.ForEach(x => {
				Assert.True(fileSystem.FileExists(x.Name));
				Assert.Equal(fileSystem.ReadFile(x.Name), x.Text);
			});
    }

	private void AssertSourceIsBinned(string backuperName) {
        var pathsBuilder = _host.Services.GetRequiredService<IPathsBuilderService>();
        var backupersPath = pathsBuilder.GetBinDirectory(backuperName);
        var backuperDirectory = _host.Services.GetRequiredService<IDirectoryInfoProvider>().FromDirectoryPath(backupersPath);
        var version = backuperDirectory.EnumerateDirectories().First();
        AssertSourceIsHere(version.FullName);
	}

	private void AssertSourceIsBackedUp(string backuperName) {
		var pathsBuilder = _host.Services.GetRequiredService<IPathsBuilderService>();
		var backupersPath = pathsBuilder.GetBackupsDirectory(backuperName);
		var backuperDirectory = _host.Services.GetRequiredService<IDirectoryInfoProvider>().FromDirectoryPath(backupersPath);
		var version = backuperDirectory.EnumerateDirectories().First();
		AssertSourceIsHere(version.FullName);
	}

	private async Task AssertBackuperDBValues(BackuperInfo expected) {
        var conn = _host.Services.GetRequiredService<IBackuperConnection>();
        Assert.True(conn.Exists(expected.Name));

        var actual = await conn.GetAsync(expected.Name);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.SourcePath, actual.SourcePath);
        Assert.Equal(expected.MaxVersions, actual.MaxVersions);
    }

	private void SetUpSource() {
		var fileSystem = _host.Services.GetRequiredService<IMockFileSystem>();
		fileSystem.Reset();
		fileSystem.CreateDirectory(_sourceDirectory);
		GenerateFiles(_sourceDirectory)
			.ForEach(x => fileSystem.CreateFile(x.Name, x.Text));
	}

}