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
            DateTime parsedTime = paths.VersionNameToDateTime(version);

            //assert
            Assert.StartsWith(paths.BackupsDirectory, version);
            Assert.NotEqual(default, parsedTime);

        }

        [Fact]
        public void GenerateCorrectVersionNumber() {
            string GetDirName(string path) => new DirectoryInfo(path).Name;
            string GetVersionString(string path) => GetDirName(path)[1..GetDirName(path).IndexOf(']')];
            int GetVersionNumber(string path) => int.Parse(GetVersionString(path));

            //arrange
            string mainDir = Path.Combine(Directory.GetCurrentDirectory(), "Versions");
            string backuperName = "nameHere";

            try {
                Paths paths = new(mainDir, backuperName);

                //act
                var noDirPathResult = paths.GenerateNewBackupVersionDirectory();
                var noDirResult = GetVersionNumber(noDirPathResult);
                Directory.CreateDirectory(noDirPathResult);
                var oneDirResult = GetVersionNumber(paths.GenerateNewBackupVersionDirectory());
                var twoDirPath = paths.GenerateNewBackupVersionDirectory();
                var twoDirResult = GetVersionNumber(twoDirPath);

                Directory.CreateDirectory(twoDirPath);
                var newPaths = new Paths(mainDir, backuperName);
                var newPathsVersionResult = GetVersionNumber(newPaths.GenerateNewBackupVersionDirectory());

                //assert
                Assert.Equal(1, noDirResult);
                Assert.Equal(2, oneDirResult);
                Assert.Equal(3, twoDirResult);
                Assert.Equal(4, newPathsVersionResult);

            } finally {

                //dispose
                Directory.Delete(mainDir, true);
            }

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
