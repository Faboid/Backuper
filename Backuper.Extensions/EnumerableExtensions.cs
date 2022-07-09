﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backuper.Extensions {
    public static class EnumerableExtensions {

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
            foreach(var item in enumerable) {
                action.Invoke(item);
            }

            return enumerable;
        }

    }
}
