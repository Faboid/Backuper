using Backuper.Abstractions;
using Backuper.Abstractions.TestingHelpers;
using Backuper.Core.Services;
using Backuper.Core.Versioning;
using Backuper.Extensions;
using Moq;

namespace Backuper.Core.Tests.Versioning {
    public class BackuperVersioningTests {

        public BackuperVersioningTests() {
            _mockFileSystem = new MockFileSystem();
            _dateTimeProvider = new DateTimeProvider();
            _directoryInfoProvider = new MockDirectoryInfoProvider(_mockFileSystem);
            _pathsBuilderService = new PathsBuilderService(_mainDirectory, _dateTimeProvider, _directoryInfoProvider);
            _sutFactory = new BackuperVersioningFactory(_pathsBuilderService, _directoryInfoProvider);
        }

        private readonly string _mainDirectory = Path.Combine(Directory.GetCurrentDirectory(), "BackuperVersioningTestsMainDirectory");
        private readonly IBackuperVersioningFactory _sutFactory;
        private readonly IDirectoryInfoProvider _directoryInfoProvider;
        private readonly IPathsBuilderService _pathsBuilderService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMockFileSystem _mockFileSystem;

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(12)]
        public void DeleteExtraVersions_DeletesExtra(int maxVersions) {
            int GetCurrentVersions(string name) 
                => _directoryInfoProvider
                    .FromDirectoryPath(_pathsBuilderService.GetBackuperDirectory(name))
                    .EnumerateDirectories()
                    .Count();

            //arrange
            ResetFileSystem();
            var backuperName = "AFittingNameForThis";
            var sut = _sutFactory.CreateVersioning(backuperName);

            Enumerable
                .Range(0, 15)
                .Select(x => _pathsBuilderService.GenerateNewBackupVersionDirectory(backuperName))
                .ForEach(x => _mockFileSystem.CreateDirectory(x));

            var startingVersiong = GetCurrentVersions(backuperName);

            //act
            sut.DeleteExtraVersions(maxVersions);

            //assert
            Assert.Equal(maxVersions, GetCurrentVersions(backuperName));

        }

        [Theory]
        [InlineData("SomeName")]
        [InlineData("AnotherName")]
        public void GenerateNewVersionWithInterface(string backuperName) {

            //arrange
            Mock<IPathsBuilderService> _mockPathsBuilderService = new();
            _mockPathsBuilderService.Setup(x => x.GetBackuperDirectory(It.IsAny<string>())).Returns<string>(x => x);
            _mockPathsBuilderService.Setup(x => x.GetBinDirectory(It.IsAny<string>())).Returns<string>(x => x);
            BackuperVersioning sut = new(backuperName, _mockPathsBuilderService.Object, _directoryInfoProvider);

            //act
            _ = sut.GenerateNewBackupVersionDirectory();

            //assert
            _mockPathsBuilderService.Verify(x => x.GenerateNewBackupVersionDirectory(It.Is(backuperName, StringComparer.Ordinal)));

        }

        private void ResetFileSystem() {
            _mockFileSystem.Reset();
            _mockFileSystem.CreateDirectory(_mainDirectory);
        }
    }
}
