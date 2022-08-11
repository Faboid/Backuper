namespace Backuper.DependencyInversion;

public interface IDateTimeProvider {
    DateTime Now { get; }
}

public class DateTimeProvider : IDateTimeProvider {
    DateTime IDateTimeProvider.Now => DateTime.Now;

}