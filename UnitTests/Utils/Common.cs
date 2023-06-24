using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Utils
{
    internal static class Common
    {
        public static string GetBasePath()
        {
            DirectoryInfo? WorkingDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
            string? ProjectDir = WorkingDir?.Parent?.Parent?.Parent?.FullName + "/PathTests";
            return ProjectDir ?? String.Empty;
        }
    }
}
