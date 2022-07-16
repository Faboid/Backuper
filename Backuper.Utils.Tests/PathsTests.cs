﻿namespace Backuper.Utils.Tests {
    public class PathsTests {

        [Theory]
        [InlineData("ghjrhwk")] //paths doesn't currently check for validity.
        [InlineData(@"D://SomePath")]
        public void GenerateCorrectDirectories(string mainDir) {

            //arrange
            string bcpName = "Heyyo";
            Paths paths = new(mainDir, bcpName);

            //act
            mainDir = Path.Combine(mainDir, "Backuper");
            var expectedBin = Path.Combine(mainDir, "Bin", bcpName);
            var expectedBackups = Path.Combine(mainDir, "Backups", bcpName);

            //assert
            Assert.Equal(expectedBin, paths.BinDirectory);
            Assert.Equal(expectedBackups, paths.BackupsDirectory);

        }

        [Fact]
        public void GenerateParseableVersionName() {

            //arrange
            string mainDir = Directory.GetCurrentDirectory();
            Paths paths = new(mainDir, "SomeName");

            //act
            var version = paths.GenerateNewBackupVersionDirectory();
            DateTime parsedTime = paths.VersionNameToDateTime(version);

            //assert
            Assert.StartsWith(paths.BackupsDirectory, version);
            Assert.NotEqual(default, parsedTime);

        }

        [Fact]
        public void GeneratePreciseVersionName() {

            //arrange
            string mainDir = Directory.GetCurrentDirectory();
            Paths paths = new(mainDir, "SomeName");
            DateTime time = DateTime.Now;

            //act
            var version = paths.GenerateNewBackupVersionDirectory(time);
            DateTime parsedTime = paths.VersionNameToDateTime(version);

            //assert
            Assert.Equal(time, parsedTime, TimeSpan.FromSeconds(5));

        }

        [Fact]
        public void GenerateValidPathName() {

            //arrange
            var invalid = Path.GetInvalidFileNameChars();
            Paths paths = new(Directory.GetCurrentDirectory(), "someName");

            //act
            var versDir = paths.GenerateNewBackupVersionDirectory();
            var isValid = new DirectoryInfo(versDir).Name.Any(x => invalid.Contains(x));

            //assert
            Assert.False(isValid);

        }
        

    }
}