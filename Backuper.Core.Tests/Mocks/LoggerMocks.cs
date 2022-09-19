using Microsoft.Extensions.Logging;
using Moq;

namespace Backuper.Core.Tests.Mocks;

public static class LoggerMocks {

    public static ILogger<T> Logger<T>() => Mock.Of<ILogger<T>>();

}