using Backuper.Core.BackuperTypes;
using Backuper.Core.Models;
using Backuper.Utils;

namespace Backuper.Core.Tests.BackuperTypes; 

public class FileBackuperTests : IDisposable {

    public FileBackuperTests() {
        //preemptive delete if the last test run didn't dispose correctly.
        if(Directory.Exists(sourceDirectory)) {
            Directory.Delete(sourceDirectory, true);
        }
        if(Directory.Exists(backuperPath)) {
            Directory.Delete(backuperPath, true);
        }

        Directory.CreateDirectory(sourceDirectory);
        File.WriteAllText(sourceFilePath, fileData);
        Directory.CreateDirectory(backuperPath);
        builder = new(backuperPath);
    }

    private const string fileName = "someFile.extension";
    private const string fileData = "Hello World!";

    private readonly static string sourceDirectory = Path.Combine(Directory.GetCurrentDirectory(), "FileBackupersTestData");
    private readonly static string backuperPath = Path.Combine(Directory.GetCurrentDirectory(), "FileBackuperTestsBackups");
    private readonly static string sourceFilePath = Path.Combine(sourceDirectory, fileName);

    private readonly PathsBuilder builder;

    [Fact]
    public async Task StartBackuperAsync_CreatesNewVersionWithCopyOfSource() {

        //arrange
        string name = "someName";
        BackuperInfo info = new(name, sourceFilePath, 3, false);
        using FileBackuper backuper = new(info, builder);
        Paths paths = builder.Build(name);

        //act
        await backuper.StartBackupAsync();
        var writtenDir = Directory.GetDirectories(paths.BackupsDirectory).First();

        //assert
        Assert.True(Directory.Exists(paths.BackupsDirectory));
        Assert.True(paths.VersionNameToDateTime(writtenDir) != default);
        Assert.True(Directory.Exists(writtenDir));
        Assert.Equal(fileData, File.ReadAllText(Path.Combine(writtenDir, fileName)));

    }

    [Fact(Skip = "Windows doesn't seem to update the file's write time, so this test will fail because of that")]
    public async Task StartBackuperAsync_BackupOnlyIfNotUpdated() {

        //arrange
        string name = "someName";
        BackuperInfo info = new(name, sourceFilePath, 3, false);
        using FileBackuper backuper = new(info, builder);
        Paths paths = builder.Build(name);

        //act
        for(int i = 0; i < 10; i++) {
            await backuper.StartBackupAsync();
        }

        await Task.Delay(TimeSpan.FromSeconds(1));
        //refresh write time
        File.Delete(sourceFilePath);
        File.WriteAllText(sourceFilePath, fileData);
        await backuper.StartBackupAsync();

        //assert
        Assert.True(Directory.Exists(paths.BackupsDirectory));
        Assert.Equal(2, Directory.GetDirectories(paths.BackupsDirectory).Length);

    }

    [Fact]
    public async Task BinBackupsAsync_MovesBackupsToBin_And_DeletesBackupsFromMainDirectory() {

        //arrange
        string name = "backuperName";
        BackuperInfo info = new(name, sourceFilePath, 3, false);
        using FileBackuper backuper = new(info, builder);
        Paths paths = builder.Build(name);
        await backuper.StartBackupAsync();

        //act
        await backuper.BinBackupsAsync();
        var writtenDir = Directory.GetDirectories(paths.BinDirectory).First();

        //assert
        Assert.False(Directory.Exists(paths.BackupsDirectory));
        Assert.True(Directory.Exists(paths.BinDirectory));
        Assert.True(paths.VersionNameToDateTime(writtenDir) != default);
        Assert.True(Directory.Exists(writtenDir));
        Assert.Equal(fileData, File.ReadAllText(Path.Combine(writtenDir, fileName)));

    }

    [Fact]
    public async Task EraseBackupsAsync_DeletesAllBackupsCorrectly() {

        //arrange
        string name = "someNameHere";
        BackuperInfo info = new(name, sourceFilePath, 3, false);
        using FileBackuper backuper = new(info, builder);
        Paths paths = builder.Build(name);

        //act
        var existed = Directory.Exists(paths.BackupsDirectory);
        await backuper.EraseBackupsAsync();

        //assert
        Assert.True(existed);
        Assert.False(Directory.Exists(paths.BackupsDirectory));

    }

    public void Dispose() {
        Directory.Delete(sourceDirectory, true);
        Directory.Delete(backuperPath, true);
    }
}
