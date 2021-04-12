using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackuperLibrary {
    public static class GlobalValues {

        public static List<Backuper> Backupers { get; set; } = GetSampleBackupers();

        private static List<Backuper> GetSampleBackupers() {
            var backupers = new List<Backuper>();
            backupers.Add(new Backuper(@"D:\Programming\Small Projects\Backuper\TemporaryTestFolder\From", "Test", 5));
            backupers.Add(new Backuper(@"D:\Programming\Small Projects\Backuper\TemporaryTestFolder\From", "SecondTest", 5));
            backupers.Add(new Backuper(@"D:\Programming\Small Projects\Backuper\TemporaryTestFolder\From", "ThirdTest", 5));
            return backupers;
        }

    }
}
