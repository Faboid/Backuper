using Backuper.Core.BackuperTypes;
using Backuper.Core.Models;
using Backuper.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backuper.Core.Tests.BackuperTypes {
    public class DirectoryBackuperTests : IDisposable {

        public DirectoryBackuperTests() {
            //preemptive delete if the last test run didn't dispose correctly.
            if(Directory.Exists(sourceData)) {
                Directory.Delete(sourceData, true);
            }
            if(Directory.Exists(backuperPath)) {
                Directory.Delete(backuperPath, true);
            }

            Directory.CreateDirectory(sourceData);
            Directory.CreateDirectory(Directory.GetParent(sourceFilePath)!.FullName);
            File.WriteAllText(sourceFilePath, fileData);
            Directory.CreateDirectory(backuperPath);
            builder = new(backuperPath);
        }

        private const string fileName = "someFile.extension";
        private const string fileData = "Hello World!";
        private const string directoryName = "SomeDirectory";

        private readonly static string sourceData = Path.Combine(Directory.GetCurrentDirectory(), "BackupersTestData");
        private readonly static string backuperPath = Path.Combine(Directory.GetCurrentDirectory(), "BackuperTestsBackups");
        private readonly static string sourceFilePath = Path.Combine(sourceData, directoryName, fileName);

        private readonly PathsBuilder builder;

        [Fact]
        public async Task StartBackuperAsync_CreatesNewVersionWithCopyOfSource() {

            //arrange
            string name = "someName";
            BackuperInfo info = new(name, sourceData, 3, false);
            DirectoryBackuper backuper = new(info, builder);
            Paths paths = builder.Build(name);

            //act
            await backuper.StartBackupAsync();
            var writtenDir = Directory.GetDirectories(paths.BackupsDirectory).First();

            //assert
            Assert.True(Directory.Exists(paths.BackupsDirectory));
            Assert.True(paths.VersionNameToDateTime(writtenDir) != default);
            Assert.True(Directory.Exists(writtenDir));
            Assert.Equal(fileData, File.ReadAllText(Path.Combine(writtenDir, directoryName, fileName)));

        }

        [Fact]
        public async Task BinBackupsAsync_MovesBackupsToBin_And_DeletesBackupsFromMainDirectory() {

            //arrange
            string name = "backuperName";
            BackuperInfo info = new(name, sourceData, 3, false);
            DirectoryBackuper backuper = new(info, builder);
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
            Assert.Equal(fileData, File.ReadAllText(Path.Combine(writtenDir, directoryName, fileName)));

        }

        [Fact]
        public async Task EraseBackupsAsync_DeletesAllBackupsCorrectly() {

            //arrange
            string name = "someNameHere";
            BackuperInfo info = new(name, sourceData, 3, false);
            DirectoryBackuper backuper = new(info, builder);
            Paths paths = builder.Build(name);

            //act
            var existed = Directory.Exists(paths.BackupsDirectory);
            await backuper.EraseBackupsAsync();

            //assert
            Assert.True(existed);
            Assert.False(Directory.Exists(paths.BackupsDirectory));

        }

        public void Dispose() {
            Directory.Delete(sourceData, true);
            Directory.Delete(backuperPath, true);
            GC.SuppressFinalize(this);
        }
    }
}
