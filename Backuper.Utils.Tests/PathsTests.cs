using System.Globalization;

namespace Backuper.Utils.Tests {
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
            var name = new DirectoryInfo(version).Name;
            DateTime parsedTime = DateTime.Parse(name);

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
            var name = new DirectoryInfo(version).Name;
            DateTimeFormatInfo dateFormat = new();
            DateTime parsedTime = DateTime.ParseExact(name, dateFormat.UniversalSortableDateTimePattern, dateFormat);

            //assert
            Assert.Equal(time, parsedTime, TimeSpan.FromSeconds(5));

        }
        

    }
}
