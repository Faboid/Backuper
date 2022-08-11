using Backuper.Core.Services;
using Backuper.Core.Versioning;
using Backuper.Utils.Wrappers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backuper.Core.Tests.Versioning {
    public class BackuperVersioningTests : IDisposable {

        public BackuperVersioningTests() {
            _dateTimeProvider = new DateTimeProvider();
            _pathsBuilderService = new PathsBuilderService(_mainDirectory, _dateTimeProvider);
            _sutFactory = new BackuperVersioningFactory(_pathsBuilderService); 
        }

        private readonly string _mainDirectory = Path.Combine(Directory.GetCurrentDirectory(), "BackuperVersioningTestsMainDirectory");
        private readonly IBackuperVersioningFactory _sutFactory;
        private readonly IPathsBuilderService _pathsBuilderService;
        private readonly IDateTimeProvider _dateTimeProvider;

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(12)]
        public void DeleteExtraVersions_DeletesExtra(int maxVersions) {
            int GetCurrentVersions(string name) 
                => Directory
                    .EnumerateDirectories(_pathsBuilderService.GetBackuperDirectory(name))
                    .Count();

            //arrange
            var backuperName = "AFittingNameForThis";
            var sut = _sutFactory.CreateVersioning(backuperName);

            _ = Enumerable
                .Range(0, 15)
                .Select(x => _pathsBuilderService.GenerateNewBackupVersionDirectory(backuperName))
                .Select(x => Directory.CreateDirectory(x))
                .ToList();

            var startingVersiong = GetCurrentVersions(backuperName);

            //act
            sut.DeleteExtraVersions(maxVersions);

            //assert
            Assert.Equal(maxVersions, GetCurrentVersions(backuperName));

            //dispose
            Directory.Delete(_pathsBuilderService.GetBackuperDirectory(backuperName), true);

        }

        [Theory]
        [InlineData("SomeName")]
        [InlineData("AnotherName")]
        public void GenerateNewVersionWithInterface(string backuperName) {

            //arrange
            Mock<IPathsBuilderService> _mockPathsBuilderService = new();
            _mockPathsBuilderService.Setup(x => x.GetBackuperDirectory(It.IsAny<string>())).Returns<string>(x => x);
            _mockPathsBuilderService.Setup(x => x.GetBinDirectory(It.IsAny<string>())).Returns<string>(x => x);
            BackuperVersioning sut = new(backuperName, _mockPathsBuilderService.Object);

            //act
            _ = sut.GenerateNewBackupVersionDirectory();

            //assert
            _mockPathsBuilderService.Verify(x => x.GenerateNewBackupVersionDirectory(It.Is(backuperName, StringComparer.Ordinal)));

        }

        public void Dispose() {
            if(Directory.Exists(_mainDirectory)) {
                Directory.Delete(_mainDirectory, true);
            }
            GC.SuppressFinalize(this);
        }
    }
}
