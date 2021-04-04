﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackuperLibrary {
    public static class PathBuilder {

        //todo - implement a way to set To's value
        public static string To { get; private set; } = @"D:\Programming\Small Projects\Backuper\TemporaryTestFolder\To"; //temporary value to test stuff

        public static string GetToPath(string name) {
            return Path.Combine(To, name);
        }

    }
}
