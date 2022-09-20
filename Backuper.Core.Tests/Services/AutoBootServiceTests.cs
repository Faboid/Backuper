using Backuper.Abstractions;
using Backuper.Core.Services;
using Moq;

namespace Backuper.Core.Tests.Services;

public class AutoBootServiceTests {

    [Fact]
    public void PointsToStartupFolder() {

        //arrange
        string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "Backuper.lnk");
        string exePath = Environment.ProcessPath!;
        var shortcutProviderMock = new Mock<IShortcutProvider>();
        shortcutProviderMock.Setup(x => x.FromShortcutPaths(It.IsAny<string>(), It.IsAny<string>())).Returns(GetShortcutMock().Object);
        
        //act
        var sut = new AutoBootService(shortcutProviderMock.Object);

        //assert
        shortcutProviderMock.Verify(x => x.FromShortcutPaths(shortcutPath, exePath));

    }

    [Fact]
    public void CreatesWhenSettingTrue() {

        //arrange
        var shortcutMock = GetShortcutMock();
        var shortcutProviderMock = new Mock<IShortcutProvider>();
        shortcutProviderMock.Setup(x => x.FromShortcutPaths(It.IsAny<string>(), It.IsAny<string>())).Returns(shortcutMock.Object);
        var sut = new AutoBootService(shortcutProviderMock.Object);

        //act
        sut.Set(true);

        //assert
        shortcutMock.Verify(x => x.Create());

    }

    [Fact]
    public void DeletesWhenSettingFalse() {

        //arrange
        var shortcutMock = GetShortcutMock();
        var shortcutProviderMock = new Mock<IShortcutProvider>();
        shortcutProviderMock.Setup(x => x.FromShortcutPaths(It.IsAny<string>(), It.IsAny<string>())).Returns(shortcutMock.Object);
        var sut = new AutoBootService(shortcutProviderMock.Object);

        //act
        sut.Set(false);

        //assert
        shortcutMock.Verify(x => x.Delete());

    }

    private static Mock<IShortcut> GetShortcutMock() {
        var shortcutMock = new Mock<IShortcut>();
        shortcutMock.Setup(x => x.SetArguments(It.IsAny<string>())).Returns(shortcutMock.Object);
        shortcutMock.Setup(x => x.SetDescription(It.IsAny<string>())).Returns(shortcutMock.Object);
        return shortcutMock;
    }

}