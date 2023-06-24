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
    public class TestsRegexDirectory
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
                SearchExpression = @".C.m",
                UseRegularExpression = true,
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
            MyTestsMethods.ContainsAndCount(resultsLvl1, "Tes<Match>tCom</Match>uni cation");

            var itemsLvl2 = itemsLvl1.ConcatAll();
            var resultsLvl2 = itemsLvl2.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Di<Match>rCom</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Di<Match>rCom</Match>dircom");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "OtherE<Match>xCom</Match> ple");

            var itemsLvl3 = itemsLvl2.ConcatAll();
            var resultsLvl3 = itemsLvl3.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl3, "OnlyOn<Match>eCom</Match>");

            var itemsLvl4 = itemsLvl3.ConcatAll();
            var resultsLvl4 = itemsLvl4.Select(a => a.NameFormatted);
            TestsRegexFilename.BasicMatch(itemsLvl3.Where(i => i.Name.EndsWith(".txt")).First().Items);

            MyTestsMethods.ContainsAndCount(resultsLvl4, "<Match>eCom</Match>.txt");

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
            MyTestsMethods.ContainsAndCount(resultsLvl1, "Tes<Match>tCom</Match>uni cation");

            var itemsLvl2 = itemsLvl1.ConcatAll();
            var resultsLvl2 = itemsLvl2.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Di<Match>rCom</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Di<Match>rCom</Match>di<Match>rcom</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "OtherE<Match>xCom</Match> ple");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "ne<Match>wcom</Match>dir");

            var itemsLvl3 = itemsLvl2.ConcatAll();
            var resultsLvl3 = itemsLvl3.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl3, "OnlyOn<Match>eCom</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl3, "Di<Match>rcom</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl3, "Di<Match>rcOm</Match>dir");

            var itemsLvl4 = itemsLvl3.ConcatAll();
            var resultsLvl4 = itemsLvl4.Select(a => a.NameFormatted);
            TestsRegexFilename.CaseSensivity(itemsLvl3.Where(i => i.Name.EndsWith(".txt")).First().Items);

            MyTestsMethods.ContainsAndCount(resultsLvl4, "<Match>eCom</Match>.txt");

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
            MyTestsMethods.ContainsAndCount(resultsLvl1, "Tes<Match>tCom</Match>uni cation");

            var itemsLvl2 = itemsLvl1.ConcatAll();
            var resultsLvl2 = itemsLvl2.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Di<Match>rCom</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Di<Match>rCom</Match>dircom");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "OtherE<Match>xCom</Match> ple");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "ne<Match>wcom</Match>dir");

            var itemsLvl3 = itemsLvl2.ConcatAll();
            var resultsLvl3 = itemsLvl3.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl3, "OnlyOn<Match>eCom</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl3, "Di<Match>rcom</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl3, "Di<Match>rcOm</Match>dir");

            var itemsLvl4 = itemsLvl3.ConcatAll();
            var resultsLvl4 = itemsLvl4.Select(a => a.NameFormatted);
            TestsRegexFilename.AllowMultiMatch(itemsLvl3.Where(i => i.Name.EndsWith(".txt")).First().Items);

            MyTestsMethods.ContainsAndCount(resultsLvl4, "<Match>eCom</Match>.txt");

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
            var itemsLvl4 = itemsLvl3.ConcatAll();
            var resultsLvl4 = itemsLvl4.Select(a => a.NameFormatted);
            TestsRegexFilename.BasicMatch(itemsLvl3.Where(i => i.Name.EndsWith(".txt")).First().Items);

            MyTestsMethods.ContainsAndCount(resultsLvl4, "<Match>eCom</Match>.txt");

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
            MyTestsMethods.ContainsAndCount(resultsLvl1, "Tes<Match>tCom</Match>uni cation");

            var itemsLvl2 = itemsLvl1.ConcatAll();
            var resultsLvl2 = itemsLvl2.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Di<Match>rCom</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Di<Match>rCom</Match>dircom");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "OtherE<Match>xCom</Match> ple");

            var itemsLvl3 = itemsLvl2.ConcatAll();
            var resultsLvl3 = itemsLvl3.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl3, "OnlyOn<Match>eCom</Match>");

            var itemsLvl4 = itemsLvl3.ConcatAll();
            var resultsLvl4 = itemsLvl4.Select(a => a.NameFormatted);
            TestsRegexFilename.BasicMatch(itemsLvl4);

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
            MyTestsMethods.ContainsAndCount(resultsLvl1, "Tes<Match>tCom</Match>uni cation");

            var itemsLvl2 = itemsLvl1.ConcatAll();
            var resultsLvl2 = itemsLvl2.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Di<Match>rCom</Match>");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "Di<Match>rCom</Match>dircom");
            MyTestsMethods.ContainsAndCount(resultsLvl2, "OtherE<Match>xCom</Match> ple");

            var itemsLvl3 = itemsLvl2.ConcatAll();
            var resultsLvl3 = itemsLvl3.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl3, "OnlyOn<Match>eCom</Match>");

            var itemsLvl4 = itemsLvl3.ConcatAll();
            var resultsLvl4 = itemsLvl4.Select(a => a.NameFormatted);
            MyTestsMethods.ContainsAndCount(resultsLvl4, "<Match>eCom</Match>.txt");

            Assert.That(itemsLvl1.Select(i => i.Count).Sum() == MyTestsMethods.Count);
        }

        [Test]
        public void TestSearchOnlyOneFile()
        {
            TestInitialization();
            searchVM.SearchConfig.Path = basePath + @"\TestComuni cation\OtherExCom ple\";
            searchVM.RunSearch();

            var itemsLvl1 = searchVM.Matches[0].Items;
            var itemsLvl2 = itemsLvl1.ConcatAll();
            var resultsLvl4 = itemsLvl2.Select(a => a.NameFormatted);
            TestsRegexFilename.BasicMatch(itemsLvl2);

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
            MyTestsMethods.ContainsAndCount(results, "OnlyOn<Match>eCom</Match>");

            Assert.That(searchVM.Matches[0].Count == MyTestsMethods.Count);
        }
    }
}