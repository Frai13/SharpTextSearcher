using SharpTextSearcher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Utils
{
    internal static class ExtensionMethods
    {
        public static ObservableCollection<DirectoryMatch> ConcatAll(this ObservableCollection<DirectoryMatch> input)
        {
            ObservableCollection<DirectoryMatch> result = new ObservableCollection<DirectoryMatch>();

            foreach (var dirs in input)
            {
                foreach (var items in dirs.Items)
                {
                    result.Add(items);
                }
            }

            return result;
        }
    }
}
