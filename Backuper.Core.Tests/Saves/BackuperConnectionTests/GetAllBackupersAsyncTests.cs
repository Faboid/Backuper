using Backuper.Core.Models;
using Backuper.Core.Saves;
using Backuper.Core.Saves.DBConnections;
using Backuper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backuper.Core.Tests.Saves.BackuperConnectionTests {
    public class GetAllBackupersAsyncTests {

        public GetAllBackupersAsyncTests() {
            dbConn = new MemoryDBConnection();
            sut = new(dbConn);
        }

        readonly MemoryDBConnection dbConn;
        readonly BackuperConnection sut;

        [Fact]
        public async Task ReturnsAllBackupers() {

            //arrange
            var currDir = Directory.GetCurrentDirectory();
            BackuperInfo first = new("FirstOne", currDir, 10, false);
            BackuperInfo second = new("Second", currDir, 2, true);
            BackuperInfo third = new("Third", currDir, 1, false);

            var expected = new BackuperInfo[] { first, second, third };
            foreach(var info in expected) {
                await dbConn.WriteAllLinesAsync(info.Name, info.ToStrings());
            }

            //act
            var actual = await sut.GetAllBackupersAsync().ToListAsync();

            //assert
            Assert.Equal(3, actual.Count);

            for(int i = 0; i < 3; i++) {
                Assert.Equal(expected[i].Name, actual[i].Name);
                Assert.Equal(expected[i].SourcePath, actual[i].SourcePath);
                Assert.Equal(expected[i].MaxVersions, actual[i].MaxVersions);
                Assert.Equal(expected[i].UpdateOnBoot, actual[i].UpdateOnBoot);
            }

        }

    }
}
