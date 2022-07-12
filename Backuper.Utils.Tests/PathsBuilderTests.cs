namespace Backuper.Utils.Tests {
    public class PathsBuilderTests {

        [Fact]
        public void CorrectDefaultPath() {

            //arrange
            var builder = new PathsBuilder();
            var expected = Directory.GetCurrentDirectory();

            //act
            var paths = builder.Build("test");

            //assert
            Assert.StartsWith(expected, paths.BackupsDirectory);

        }

        [Fact]
        public void CorrectCustomPath() {

            //arrange
            var expected = @"C:\";
            var builder = new PathsBuilder(expected);

            //act
            var paths = builder.Build("test");

            //assert
            Assert.StartsWith(expected, paths.BackupsDirectory);

        }

        [Fact]
        public void ThrowsWhenDirectoryDoesNotExist() {
            Assert.Throws<ArgumentException>(() => { new PathsBuilder(null!); });
            Assert.Throws<ArgumentException>(() => { new PathsBuilder("    "); });
            Assert.Throws<ArgumentException>(() => { new PathsBuilder("someval"); });
        }

    }
}
