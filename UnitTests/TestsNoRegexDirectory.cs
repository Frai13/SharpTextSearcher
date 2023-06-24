using SharpTextSearcher.Core;
using SharpTextSearcher.Model;
using SharpTextSearcher.ViewModel;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Windows;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnitTests.Utils;
using NUnit.Framework.Interfaces;

namespace UnitTests
{
    public class TestsNoRegexDirectory
    {
        private SearchViewModel searchVM = new SearchViewModel();
        private string basePath = Common.GetBasePath() ?? "";

        public void TestInitialization()
        {
            Assert.That(basePath != "");

            MyTestsMethods.Initialize();

            searchVM.SearchConfig = new SearchConfig()
            {
                Path = basePath,
                SearchExpression = @"Com",
                UseRegularExpression = false,
                CaseSensivity = true,
                AllowMultiMatch = true,
                SearchInDirectoryNames = true,
                SearchInFileNames = true,
                SearchInsideFiles = true,
            };
        }

        [Test]
        public void TestBasicMatch()
        {
            TestInitialization();
            searchVM.RunSearch();

            var itemsLvl1 = searchVM.Matches[0].Items;
            var resultsLvl1 = itemsLvl1.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl1, "Test<Match>Com</Match>uni cation");

            var itemsLvl2 = itemsLvl1.ConcatAll();
            var resultsLvl2 = itemsLvl2.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl2, "<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Dir<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Dir<Match>Com</Match>dircom");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "OtherEx<Match>Com</Match> ple");

            var itemsLvl3 = itemsLvl2.ConcatAll();
            var resultsLvl3 = itemsLvl3.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl3, "OnlyOne<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl3, "<Match>Com</Match>.txt");

            var itemsLvl4 = itemsLvl3.ConcatAll();
            var resultsLvl4 = itemsLvl4.Select(a => a.NameFormatted);
            TestsNoRegexFilename.BasicMatch(itemsLvl3.Where(i => i.Name.EndsWith(".txt")).First().Items);

            MyTestsMethods.ContainsAndCount(resultsLvl4, "e<Match>Com</Match>.txt");

            Assert.That(itemsLvl1.Select(i => i.Count).Sum() == MyTestsMethods.Count);
        }

        [Test]
        public void TestCaseSensivity()
        {
            TestInitialization();
            searchVM.SearchConfig.CaseSensivity = false;
            searchVM.RunSearch();

            var itemsLvl1 = searchVM.Matches[0].Items;
            var resultsLvl1 = itemsLvl1.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl1, "Test<Match>Com</Match>uni cation");

            var itemsLvl2 = itemsLvl1.ConcatAll();
            var resultsLvl2 = itemsLvl2.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl2, "<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Dir<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Dir<Match>Com</Match>dir<Match>com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "OtherEx<Match>Com</Match> ple");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "new<Match>com</Match>dir");

            var itemsLvl3 = itemsLvl2.ConcatAll();
            var resultsLvl3 = itemsLvl3.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl3, "OnlyOne<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl3, "<Match>com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl3, "Dir<Match>com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl3, "Dir<Match>cOm</Match>dir");
            MyTestsMethods.ContainsAndCount(resultsLvl3, "<Match>Com</Match>.txt");

            var itemsLvl4 = itemsLvl3.ConcatAll();
            var resultsLvl4 = itemsLvl4.Select(a => a.NameFormatted);
            TestsNoRegexFilename.CaseSensivity(itemsLvl3.Where(i => i.Name.EndsWith(".txt")).First().Items);

            MyTestsMethods.ContainsAndCount(resultsLvl4, "e<Match>Com</Match>.txt");

            Assert.That(itemsLvl1.Select(i => i.Count).Sum() == MyTestsMethods.Count);
        }

        [Test]
        public void TestAllowMultiMatch()
        {
            TestInitialization();
            searchVM.SearchConfig.CaseSensivity = false;
            searchVM.SearchConfig.AllowMultiMatch = false;
            searchVM.RunSearch();

            var itemsLvl1 = searchVM.Matches[0].Items;
            var resultsLvl1 = itemsLvl1.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl1, "Test<Match>Com</Match>uni cation");

            var itemsLvl2 = itemsLvl1.ConcatAll();
            var resultsLvl2 = itemsLvl2.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl2, "<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Dir<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Dir<Match>Com</Match>dircom");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "OtherEx<Match>Com</Match> ple");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "new<Match>com</Match>dir");

            var itemsLvl3 = itemsLvl2.ConcatAll();
            var resultsLvl3 = itemsLvl3.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl3, "OnlyOne<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl3, "<Match>com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl3, "Dir<Match>com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl3, "Dir<Match>cOm</Match>dir");
            MyTestsMethods.ContainsAndCount(resultsLvl3, "<Match>Com</Match>.txt");

            var itemsLvl4 = itemsLvl3.ConcatAll();
            var resultsLvl4 = itemsLvl4.Select(a => a.NameFormatted);
            TestsNoRegexFilename.AllowMultiMatch(itemsLvl3.Where(i => i.Name.EndsWith(".txt")).First().Items);

            MyTestsMethods.ContainsAndCount(resultsLvl4, "e<Match>Com</Match>.txt");

            Assert.That(itemsLvl1.Select(i => i.Count).Sum() == MyTestsMethods.Count);
        }

        [Test]
        public void TestSearchInDirectoryNamesMatch()
        {
            TestInitialization();
            searchVM.SearchConfig.SearchInDirectoryNames = false;
            searchVM.RunSearch();

            var itemsLvl1 = searchVM.Matches[0].Items;
            var itemsLvl2 = itemsLvl1.ConcatAll();
            var itemsLvl3 = itemsLvl2.ConcatAll();
            var resultsLvl3 = itemsLvl3.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl3, "<Match>Com</Match>.txt");

            var itemsLvl4 = itemsLvl3.ConcatAll();
            var resultsLvl4 = itemsLvl4.Select(a => a.NameFormatted);
            TestsNoRegexFilename.BasicMatch(itemsLvl3.Where(i => i.Name.EndsWith(".txt")).First().Items);

            MyTestsMethods.ContainsAndCount(resultsLvl4, "e<Match>Com</Match>.txt");

            Assert.That(itemsLvl1.Select(i => i.Count).Sum() == MyTestsMethods.Count);
        }

        [Test]
        public void TestSearchInFileNamesMatch()
        {
            TestInitialization();
            searchVM.SearchConfig.SearchInFileNames = false;
            searchVM.RunSearch();

            var itemsLvl1 = searchVM.Matches[0].Items;
            var resultsLvl1 = itemsLvl1.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl1, "Test<Match>Com</Match>uni cation");

            var itemsLvl2 = itemsLvl1.ConcatAll();
            var resultsLvl2 = itemsLvl2.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl2, "<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Dir<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Dir<Match>Com</Match>dircom");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "OtherEx<Match>Com</Match> ple");

            var itemsLvl3 = itemsLvl2.ConcatAll();
            var resultsLvl3 = itemsLvl3.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl3, "OnlyOne<Match>Com</Match>");

            var itemsLvl4 = itemsLvl3.ConcatAll();
            var resultsLvl4 = itemsLvl4.Select(a => a.NameFormatted);
            TestsNoRegexFilename.BasicMatch(itemsLvl3.Where(i => i.Name.EndsWith(".txt")).First().Items);

            Assert.That(itemsLvl1.Select(i => i.Count).Sum() == MyTestsMethods.Count);
        }

        [Test]
        public void TestSearchInsideFilesMatch()
        {
            TestInitialization();
            searchVM.SearchConfig.SearchInsideFiles = false;
            searchVM.RunSearch();

            var itemsLvl1 = searchVM.Matches[0].Items;
            var resultsLvl1 = itemsLvl1.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl1, "Test<Match>Com</Match>uni cation");

            var itemsLvl2 = itemsLvl1.ConcatAll();
            var resultsLvl2 = itemsLvl2.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl2, "<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Dir<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Dir<Match>Com</Match>dircom");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "OtherEx<Match>Com</Match> ple");

            var itemsLvl3 = itemsLvl2.ConcatAll();
            var resultsLvl3 = itemsLvl3.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl3, "OnlyOne<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl3, "<Match>Com</Match>.txt");

            var itemsLvl4 = itemsLvl3.ConcatAll();
            var resultsLvl4 = itemsLvl4.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl4, "e<Match>Com</Match>.txt");

            Assert.That(itemsLvl1.Select(i => i.Count).Sum() == MyTestsMethods.Count);
        }

        [Test]
        public void TestSearchOnlyOneFile()
        {
            TestInitialization();
            searchVM.SearchConfig.Path = basePath + @"\TestComuni cation\OtherExCom ple\";
            searchVM.RunSearch();

            var itemsLvl1 = searchVM.Matches[0].Items;
            var resultsLvl1 = itemsLvl1.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl1, "<Match>Com</Match>.txt");

            var itemsLvl2 = itemsLvl1.ConcatAll();
            var resultsLvl4 = itemsLvl2.Select(a => a.NameFormatted);
            TestsNoRegexFilename.BasicMatch(itemsLvl2);

            Assert.That(itemsLvl1.Select(i => i.Count).Sum() == MyTestsMethods.Count);
        }

        [Test]
        public void TestSearchOnlyOneDir()
        {
            TestInitialization();
            searchVM.SearchConfig.Path = basePath + @"\TestComuni cation\DirComdircom";
            searchVM.RunSearch();

            var results = searchVM.Matches[0].Items.Select(a => a.NameFormatted);

            Assert.That(results.Count() == 1);
            MyTestsMethods.ContainsAndCount(results, "OnlyOne<Match>Com</Match>");

            Assert.That(searchVM.Matches[0].Count == MyTestsMethods.Count);
        }
    }
}