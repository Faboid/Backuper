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
            File.WriteAllText(sourceFilePath, fileData);
            Directory.CreateDirectory(backuperPath);
            builder = new(backuperPath);
        }

        private const string fileName = "someFile.extension";
        private const string fileData = "Hello World!";

        private readonly static string sourceData = Path.Combine(Directory.GetCurrentDirectory(), "BackupersTestData");
        private readonly static string backuperPath = Path.Combine(Directory.GetCurrentDirectory(), "BackuperTestsBackups");
        private readonly static string sourceFilePath = Path.Combine(sourceData, fileName);

        private readonly PathsBuilder builder;

        //todo - implement testing of directories as well
        [Fact]
        public async Task BackupsCorrectly() {

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
            Assert.Equal(fileData, File.ReadAllText(Path.Combine(writtenDir, fileName)));

        }

        public void Dispose() {
            Directory.Delete(sourceData, true);
            Directory.Delete(backuperPath, true);
            GC.SuppressFinalize(this);
        }
    }
}
