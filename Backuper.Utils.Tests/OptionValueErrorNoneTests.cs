using Backuper.TestUtils;
using Backuper.Utils.Options;
using System.Reflection;
using Xunit;

namespace Backuper.Utils.Tests {
    public class OptionValueErrorNoneTests {

        private static IOption<TValue, TError> GetUnderlyingOption<TValue, TError>(Option<TValue, TError> option) {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var field = option.GetType().GetField("_option", flags);
            var output = field!.GetValue(option);
            return (IOption<TValue, TError>)output!;
        }

        [Fact]
        public void BindMixAndMatches() {

            //arrange
            Option<int, string> option = new(5);
            string errorMessage = "Can't be zero.";

            //act
            var firstOption = option.Bind(x => new Option<int, string>(x - 5));
            var firstRes = firstOption.Or(100);
            var none = option.Bind(x => new Option<int, string>());
            var noneTwo = none.Bind(x => new Option<bool, string>(true));
            var error = firstOption.Bind(x => {
                if(x == 0) {
                    return new Option<int, string>(errorMessage);
                }

                OrderChecker.Never();
                return new Option<int, string>(5 / x);
            });

            //assert

            option.Is(OptionResult.Some);
            firstOption.Is(OptionResult.Some);
            none.Is(OptionResult.None);
            noneTwo.Is(OptionResult.None);
            error.Is(OptionResult.Error);

            Assert.Equal(0, firstRes);
            Assert.Equal(20, none.Or(20));
            Assert.Equal(50, error.Or(50));
            Assert.Equal(errorMessage, error.OrError("sup"));

            Assert.IsType<None<int, string>>(GetUnderlyingOption(none));
            Assert.IsType<None<bool, string>>(GetUnderlyingOption(noneTwo));
            Assert.IsType<Error<int, string>>(GetUnderlyingOption(error));

        }

        [Fact]
        public async Task BindAsyncMixAndMatches() {

            //arrange
            Option<int, string> option = new(5);
            string errorMessage = "Can't be zero.";

            //act
            var firstOption = await option.BindAsync(x => Task.FromResult(new Option<int, string>(x - 5)));
            var firstRes = firstOption.Or(100);
            var none = await option.BindAsync(x => Task.FromResult(new Option<int, string>()));
            var noneTwo = await none.BindAsync(x => Task.FromResult(new Option<bool, string>(true)));
            var error = await firstOption.BindAsync(x => {
                if(x == 0) {
                    return Task.FromResult(new Option<int, string>(errorMessage));
                }

                OrderChecker.Never();
                throw new Exception();
            });

            //assert

            option.Is(OptionResult.Some);
            firstOption.Is(OptionResult.Some);
            none.Is(OptionResult.None);
            noneTwo.Is(OptionResult.None);
            error.Is(OptionResult.Error);

            Assert.Equal(0, firstRes);
            Assert.Equal(20, none.Or(20));
            Assert.Equal(50, error.Or(50));

            Assert.IsType<None<int, string>>(GetUnderlyingOption(none));
            Assert.IsType<None<bool, string>>(GetUnderlyingOption(noneTwo));
            Assert.IsType<Error<int, string>>(GetUnderlyingOption(error));

        }

        public static IEnumerable<object[]> SomeData() {
            static object[] NewObj(int expected, Option<int, string> option) => new object[] { expected, option };

            int expected = 5;
            int value = expected - 2;
            Option<int, string> option = new(value);
            Some<int, string> some = new(value);

            //explicit
            yield return NewObj(expected, option);
            yield return NewObj(expected, Option<int, string>.Some(value));
            yield return NewObj(expected, Option.Some<int, string>(value));
            //implicit
            yield return NewObj(expected, value);
            yield return NewObj(expected, some);

        }

        [Theory]
        [MemberData(nameof(SomeData))]
        public void SomeWorksCorrectly(int expected, Option<int, string> option) {

            //act
            var result = option.Match(
                some => some + 2,
                error => { OrderChecker.Never(); return 5; },
                () => { OrderChecker.Never(); return 2; }
            );

            //assert
            Assert.Equal(expected, result);
            Assert.Equal(expected, option.Or(2) + 2);
            Assert.Equal("nope", option.OrError("nope"));
            option.Is(OptionResult.Some);
            Assert.IsType<Some<int, string>>(GetUnderlyingOption(option));

        }

        public static IEnumerable<object[]> ErrorData() {
            static object[] NewObj(string error, Option<int, string> option) => new object[] { error, option };

            string errorMessage = "Failed!";
            Option<int, string> option = new(errorMessage);
            Error<int, string> error = new(errorMessage);

            //explicit
            yield return NewObj(errorMessage, option);
            yield return NewObj(errorMessage, Option<int, string>.Error(errorMessage));
            yield return NewObj(errorMessage, Option.Error<int, string>(errorMessage));
            //implicit
            yield return NewObj(errorMessage, errorMessage);
            yield return NewObj(errorMessage, error);

        }

        [Theory]
        [MemberData(nameof(ErrorData))]
        public void ErrorWorksCorrectly(string errorMessage, Option<int, string> option) {

            //act
            var result = option.Match(
                some => { OrderChecker.Never(); return "some"; },
                error => error,
                () => { OrderChecker.Never(); return "none"; }
            );
            var bind = option.Bind<bool>(x => throw new NotImplementedException());

            //assert
            Assert.Equal(errorMessage, result);
            Assert.Equal(errorMessage, option.OrError("nope"));
            option.Is(OptionResult.Error);
            Assert.IsType<Error<bool, string>>(GetUnderlyingOption(bind));

        }

        public static IEnumerable<object[]> NoneData() {
            static object[] NewObj(Option<string?, bool?> option) => new object[] { option };

            Option<string?, bool?> option = new();
            None<string?, bool?> none = new();
            string? val = null;

            //explicit
            yield return NewObj(option);
            yield return NewObj(Option<string?, bool?>.None());
            yield return NewObj(Option.None<string?, bool?>());

            //implicit
            yield return NewObj(default);
            yield return NewObj(none);
            yield return NewObj(val);

            //pass null to some & error
            yield return NewObj(new Option<string?, bool?>(value: null));
            yield return NewObj(new Option<string?, bool?>(error: null));
            yield return NewObj(Option.Some<string?, bool?>(null));
            yield return NewObj(Option<string?, bool?>.Some(null));
            yield return NewObj(Option.Error<string?, bool?>(null));
            yield return NewObj(Option<string?, bool?>.Error(null));

        }

        [Theory]
        [MemberData(nameof(NoneData))]
        public void NoneWorksCorrectly(Option<string?, bool?> option) {

            //act
            var result = option.Match(
                some => { OrderChecker.Never(); return false; },
                error => { OrderChecker.Never(); return false; },
                () => { return true; }
            );
            var bind = option.Bind(x => new Option<int, bool?>(int.Parse(x!)));

            //assert
            Assert.True(result);
            Assert.Equal("stuff", option.Or("stuff"));
            Assert.Equal(true, option.OrError(true));
            option.Is(OptionResult.None);
            Assert.IsType<None<int, bool?>>(GetUnderlyingOption(bind));

        }

    }
}
