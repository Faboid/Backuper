using Backuper.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backuper.Core.Tests.Models {
    public class BackuperInfoTests {

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void ThrowsOnZeroOrNegativeMaxVersions(int maxVersions) {
            Assert.Throws<ArgumentOutOfRangeException>(() => new BackuperInfo("ValidName", @"C:\ValidPath", maxVersions));
        }

        [Theory]
        [InlineData("name", @"T:\path", 5)]
        [InlineData("SomeName", @"D:\Here", 1)]
        [InlineData("AnotherName", @"C:\Place", 500)]
        public void ToStringAndParse(string name, string path, int maxVer) {

            //arrange
            BackuperInfo info = new(name, path, maxVer);

            //act
            var asString = info.ToString();
            var infoParsed = BackuperInfo.Parse(asString);

            //assert
            Assert.Equal(info.Name, infoParsed.Name);
            Assert.Equal(info.SourcePath, infoParsed.SourcePath);
            Assert.Equal(info.MaxVersions, infoParsed.MaxVersions);

        }

    }
}
