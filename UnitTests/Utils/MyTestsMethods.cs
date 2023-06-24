using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UnitTests.Utils
{
    internal static class MyTestsMethods
    {
        private static int count;
        public static int Count { get => count; }

        public static void Initialize()
        {
            count = 0;
        }

        public static void ContainsAndCount(IEnumerable<string> results, string line)
        {
            Assert.That(results.Contains(line));

            int countMatch = Regex.Matches(line, "<Match>").Count;
            int countEndMatch = Regex.Matches(line, "</Match>").Count;
            Assert.That(countMatch == countEndMatch);

            count += countMatch;
        }
    }
}
