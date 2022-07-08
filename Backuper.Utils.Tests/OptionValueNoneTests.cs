using Backuper.TestUtils;
using Backuper.Utils.Options;
using System.Reflection;
using Xunit;

namespace Backuper.Utils.Tests {
    public class OptionValueNoneTests {

        private static IOption<TValue> GetUnderlyingOption<TValue>(Option<TValue> option) {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var field = option.GetType().GetField("_option", flags);
            var output = field!.GetValue(option);
            return (IOption<TValue>)output!;
        }

        [Fact]
        public void BindMixAndMatches() {

            //arrange
            Option<int> option = new(5);

            //act
            var firstOption = option.Bind(x => new Option<int>(x - 5));
            var firstRes = firstOption.Or(100);
            var none = option.Bind(x => new Option<int>());
            var noneTwo = none.Bind(x => new Option<bool>(true));

            //assert
            option.Is(OptionResult.Some);
            firstOption.Is(OptionResult.Some);
            none.Is(OptionResult.None);
            noneTwo.Is(OptionResult.None);

            Assert.Equal(0, firstRes);
            Assert.Equal(20, none.Or(20));

            Assert.IsType<None<int>>(GetUnderlyingOption(none));
            Assert.IsType<None<bool>>(GetUnderlyingOption(noneTwo));

        }

        [Fact]
        public async Task BindAsyncMixAndMatches() {

            //arrange
            Option<int> option = new(5);

            //act
            var firstOption = await option.BindAsync(x => Task.FromResult(new Option<int>(x - 5)));
            var firstRes = firstOption.Or(100);
            var none = await option.BindAsync(x => Task.FromResult(new Option<int>()));
            var noneTwo = await none.BindAsync(x => Task.FromResult(new Option<bool>(true)));

            //assert
            option.Is(OptionResult.Some);
            firstOption.Is(OptionResult.Some);
            none.Is(OptionResult.None);
            noneTwo.Is(OptionResult.None);

            Assert.Equal(0, firstRes);
            Assert.Equal(20, none.Or(20));

            Assert.IsType<None<int>>(GetUnderlyingOption(none));
            Assert.IsType<None<bool>>(GetUnderlyingOption(noneTwo));

        }

        public static IEnumerable<object[]> SomeData() {
            static object[] NewObj(int expected, Option<int> option) => new object[] { expected, option };

            int expected = 5;
            int value = expected - 2;
            Option<int> option = new(value);
            Some<int> some = new(value);

            //explicit
            yield return NewObj(expected, option);
            yield return NewObj(expected, Option<int>.Some(value));
            yield return NewObj(expected, Option.Some(value));
            //implicit
            yield return NewObj(expected, value);
            yield return NewObj(expected, some);

        }

        [Theory]
        [MemberData(nameof(SomeData))]
        public void SomeWorksCorrectly(int expected, Option<int> option) {

            //act
            var result = option.Match(
                some => some + 2,
                () => { OrderChecker.Never(); return 2; }
            );

            //assert
            Assert.Equal(expected, result);
            option.Is(OptionResult.Some);
            Assert.IsType<Some<int>>(GetUnderlyingOption(option));

        }

        public static IEnumerable<object[]> NoneData() {
            static object[] NewObj(Option<string?> option) => new object[] { option };

            Option<string?> option = new();
            None<string?> none = new();
            string? val = null;

            //explicit
            yield return NewObj(option);
            yield return NewObj(Option<string?>.None());
            yield return NewObj(Option.None<string?>());

            //implicit
            yield return NewObj(default);
            yield return NewObj(none);
            yield return NewObj(val);

            //pass null to some
            yield return NewObj(new Option<string?>(value: null));
            yield return NewObj(Option.Some<string?>(null));
            yield return NewObj(Option<string?>.Some(null));

        }

        [Theory]
        [MemberData(nameof(NoneData))]
        public void NoneWorksCorrectly(Option<string?> option) {

            //act
            var result = option.Match(
                some => { OrderChecker.Never(); return false; },
                () => { return true; }
            );
            var bind = option.Bind(x => new Option<int>(int.Parse(x!)));

            //assert
            Assert.True(result);
            option.Is(OptionResult.None);
            Assert.IsType<None<int>>(GetUnderlyingOption(bind));

        }

    }
}
