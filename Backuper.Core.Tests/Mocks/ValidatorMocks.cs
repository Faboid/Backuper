using Backuper.Core.Models;
using Backuper.Core.Validation;
using Moq;

namespace Backuper.Core.Tests.Mocks;

public static class ValidatorMocks {

    public static IBackuperValidator GetAlwaysValid() {
        var mockedValidator = new Mock<IBackuperValidator>();
        mockedValidator.Setup(x => x.IsValid(It.IsAny<BackuperInfo>())).Returns(BackuperValid.Valid);
        return mockedValidator.Object;
    }

}