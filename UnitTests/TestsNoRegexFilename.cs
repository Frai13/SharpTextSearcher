using SharpTextSearcher.Core;
using SharpTextSearcher.Model;
using SharpTextSearcher.ViewModel;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Windows;
using System.Reflection;
using UnitTests.Utils;
using System.Collections.ObjectModel;
using NUnit.Framework.Interfaces;

namespace UnitTests
{
    public class TestsNoRegexFilename
    {
        private SearchViewModel searchVM = new SearchViewModel();
        private string basePath = Common.GetBasePath() ?? "";

        public void TestInitialization()
        {
            Assert.That(basePath != "");

            MyTestsMethods.Initialize();

            searchVM.SearchConfig = new SearchConfig()
            {
                Path = basePath + "/TestComuni cation/OtherExCom ple/Com.txt",
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

            BasicMatch(searchVM.Matches[0].Items);
        }

        public static void BasicMatch(ObservableCollection<DirectoryMatch> items)
        {
            Assert.That(items.Where(i => i.MatchType != SharpTextSearcher.Model.MatchType.LINE).Count() == 0);

            var results = items.Select(a => a.NameFormatted);

            int initValue = MyTestsMethods.Count;

            MyTestsMethods.ContainsAndCount(results, "1\tHello<Match>Com</Match>thisiscomexample com<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(results, "3\t <Match>Com</Match>com ");
            MyTestsMethods.ContainsAndCount(results, "4\tcom<Match>Com</Match> purposes");
            MyTestsMethods.ContainsAndCount(results, "6\tabD<\0Match>de<Match>Com</Match>f<\0/Match>asdasd");
            MyTestsMethods.ContainsAndCount(results, "7\tab<Match>Com</Match>c<\0Match>def<\0/Match>asdasd");
            MyTestsMethods.ContainsAndCount(results, "8\tabd<\0Match>def<\0/Match>a<Match>Com</Match>sdasd");
            MyTestsMethods.ContainsAndCount(results, "9\tabDd<B<Match>Com</Match>old>def<\0/Match>as<Match>Com</Match>das");
            MyTestsMethods.ContainsAndCount(results, "10\tdabD<\0Match>def</Bo<Match>Com</Match>ld>asda<Match>Com</Match>sd");
            MyTestsMethods.ContainsAndCount(results, "11\tabd<\0Match>def<\0/Match>asdasdabc<\0Match>def<\0/Match>asd<Match>Com</Match>asd");
            MyTestsMethods.ContainsAndCount(results, "12\tab<\0Match>def<\0/Match>asd<Match>Com</Match>asdabc<\0Match>def<\0/Match>asd<Match>Com</Match>asd");
            MyTestsMethods.ContainsAndCount(results, "13\t<Match>Com</Match>adadasbda");
            MyTestsMethods.ContainsAndCount(results, "14\tdadsasdcasc<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(results, "15\t<Match>Com</Match>ASdadsad<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(results, "16\t<Match>Com</Match>ada<\0Match>dasbda");
            MyTestsMethods.ContainsAndCount(results, "17\tdadsas<\0Match>dcasc<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(results, "18\t<Match>Com</Match>ASd<\0Match>adsad<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(results, "19\taa\0<Match>Com</Match>\0bb");
            MyTestsMethods.ContainsAndCount(results, "21\t\0<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(results, "22\t<Match>Com</Match>\0");

            Assert.That(items.Select(i => i.Count).Sum() == MyTestsMethods.Count - initValue);
        }

        [Test]
        public void TestCaseSensivity()
        {
            TestInitialization();
            searchVM.SearchConfig.CaseSensivity = false;
            searchVM.RunSearch();

            CaseSensivity(searchVM.Matches[0].Items);
        }

        public static void CaseSensivity(ObservableCollection<DirectoryMatch> items)
        {
            Assert.That(items.Where(i => i.MatchType != SharpTextSearcher.Model.MatchType.LINE).Count() == 0);

            var results = items.Select(a => a.NameFormatted);

            int initValue = MyTestsMethods.Count;

            MyTestsMethods.ContainsAndCount(results, "1\tHello<Match>Com</Match>thisis<Match>com</Match>example <Match>com</Match><Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(results, "2\tbut onlyfor <Match>com</Match> testing");
            MyTestsMethods.ContainsAndCount(results, "3\t <Match>Com</Match><Match>com</Match> ");
            MyTestsMethods.ContainsAndCount(results, "4\t<Match>com</Match><Match>Com</Match> purposes");
            MyTestsMethods.ContainsAndCount(results, "6\tabD<\0Match>de<Match>Com</Match>f<\0/Match>asdasd");
            MyTestsMethods.ContainsAndCount(results, "7\tab<Match>Com</Match>c<\0Match>def<\0/Match>asdasd");
            MyTestsMethods.ContainsAndCount(results, "8\tabd<\0Match>def<\0/Match>a<Match>Com</Match>sdasd");
            MyTestsMethods.ContainsAndCount(results, "9\tabDd<B<Match>Com</Match>old>def<\0/Match>as<Match>Com</Match>das");
            MyTestsMethods.ContainsAndCount(results, "10\tdabD<\0Match>def</Bo<Match>Com</Match>ld>asda<Match>Com</Match>sd");
            MyTestsMethods.ContainsAndCount(results, "11\tabd<\0Match>def<\0/Match>asdasdabc<\0Match>def<\0/Match>asd<Match>Com</Match>asd");
            MyTestsMethods.ContainsAndCount(results, "12\tab<\0Match>def<\0/Match>asd<Match>Com</Match>asdabc<\0Match>def<\0/Match>asd<Match>Com</Match>asd");
            MyTestsMethods.ContainsAndCount(results, "13\t<Match>Com</Match>adadasbda");
            MyTestsMethods.ContainsAndCount(results, "14\tdadsasdcasc<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(results, "15\t<Match>Com</Match>ASdadsad<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(results, "16\t<Match>Com</Match>ada<\0Match>dasbda");
            MyTestsMethods.ContainsAndCount(results, "17\tdadsas<\0Match>dcasc<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(results, "18\t<Match>Com</Match>ASd<\0Match>adsad<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(results, "19\taa\0<Match>Com</Match>\0bb");
            MyTestsMethods.ContainsAndCount(results, "21\t\0<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(results, "22\t<Match>Com</Match>\0");

            Assert.That(items.Select(i => i.Count).Sum() == MyTestsMethods.Count - initValue);
        }

        [Test]
        public void TestAllowMultiMatch()
        {
            TestInitialization();
            searchVM.SearchConfig.CaseSensivity = false;
            searchVM.SearchConfig.AllowMultiMatch = false;
            searchVM.RunSearch();

            AllowMultiMatch(searchVM.Matches[0].Items);
        }

        public static void AllowMultiMatch(ObservableCollection<DirectoryMatch> items)
        {
            Assert.That(items.Where(i => i.MatchType != SharpTextSearcher.Model.MatchType.LINE).Count() == 0);

            var results = items.Select(a => a.NameFormatted);

            int initValue = MyTestsMethods.Count;

            MyTestsMethods.ContainsAndCount(results, "1\tHello<Match>Com</Match>thisiscomexample comCom");
            MyTestsMethods.ContainsAndCount(results, "2\tbut onlyfor <Match>com</Match> testing");
            MyTestsMethods.ContainsAndCount(results, "3\t <Match>Com</Match>com ");
            MyTestsMethods.ContainsAndCount(results, "4\t<Match>com</Match>Com purposes");
            MyTestsMethods.ContainsAndCount(results, "6\tabD<\0Match>de<Match>Com</Match>f<\0/Match>asdasd");
            MyTestsMethods.ContainsAndCount(results, "7\tab<Match>Com</Match>c<\0Match>def<\0/Match>asdasd");
            MyTestsMethods.ContainsAndCount(results, "8\tabd<\0Match>def<\0/Match>a<Match>Com</Match>sdasd");
            MyTestsMethods.ContainsAndCount(results, "9\tabDd<B<Match>Com</Match>old>def<\0/Match>asComdas");
            MyTestsMethods.ContainsAndCount(results, "10\tdabD<\0Match>def</Bo<Match>Com</Match>ld>asdaComsd");
            MyTestsMethods.ContainsAndCount(results, "11\tabd<\0Match>def<\0/Match>asdasdabc<\0Match>def<\0/Match>asd<Match>Com</Match>asd");
            MyTestsMethods.ContainsAndCount(results, "12\tab<\0Match>def<\0/Match>asd<Match>Com</Match>asdabc<\0Match>def<\0/Match>asdComasd");
            MyTestsMethods.ContainsAndCount(results, "13\t<Match>Com</Match>adadasbda");
            MyTestsMethods.ContainsAndCount(results, "14\tdadsasdcasc<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(results, "15\t<Match>Com</Match>ASdadsadCom");
            MyTestsMethods.ContainsAndCount(results, "16\t<Match>Com</Match>ada<\0Match>dasbda");
            MyTestsMethods.ContainsAndCount(results, "17\tdadsas<\0Match>dcasc<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(results, "18\t<Match>Com</Match>ASd<\0Match>adsadCom");
            MyTestsMethods.ContainsAndCount(results, "19\taa\0<Match>Com</Match>\0bb");
            MyTestsMethods.ContainsAndCount(results, "21\t\0<Match>Com</Match>");
            MyTestsMethods.ContainsAndCount(results, "22\t<Match>Com</Match>\0");

            Assert.That(items.Select(i => i.Count).Sum() == MyTestsMethods.Count - initValue);
        }

        [Test]
        public void TestSearchInDirectoryNamesMatch()
        {
            // Should not affect in filename mode
            TestInitialization();
            searchVM.SearchConfig.SearchInDirectoryNames = false;
            searchVM.RunSearch();
            BasicMatch(searchVM.Matches[0].Items);

            searchVM.SearchConfig.CaseSensivity = false;
            searchVM.RunSearch();
            CaseSensivity(searchVM.Matches[0].Items);

            searchVM.SearchConfig.AllowMultiMatch = false;
            searchVM.RunSearch();
            AllowMultiMatch(searchVM.Matches[0].Items);
        }

        [Test]
        public void TestSearchInFileNamesMatch()
        {
            // Should not affect in filename mode
            TestInitialization();
            searchVM.SearchConfig.SearchInFileNames = false;
            searchVM.RunSearch();
            BasicMatch(searchVM.Matches[0].Items);

            searchVM.SearchConfig.CaseSensivity = false;
            searchVM.RunSearch();
            CaseSensivity(searchVM.Matches[0].Items);

            searchVM.SearchConfig.AllowMultiMatch = false;
            searchVM.RunSearch();
            AllowMultiMatch(searchVM.Matches[0].Items);
        }

        [Test]
        public void TestSearchInsideFilesMatch()
        {
            // Should not affect in filename mode
            TestInitialization();
            searchVM.SearchConfig.SearchInsideFiles = false;
            searchVM.RunSearch();
            BasicMatch(searchVM.Matches[0].Items);

            searchVM.SearchConfig.CaseSensivity = false;
            searchVM.RunSearch();
            CaseSensivity(searchVM.Matches[0].Items);

            searchVM.SearchConfig.AllowMultiMatch = false;
            searchVM.RunSearch();
            AllowMultiMatch(searchVM.Matches[0].Items);
        }

        [Test]
        public void TestNullCharacterInsideMatch()
        {
            Assert.That(basePath != "");

            MyTestsMethods.Initialize();

            searchVM.SearchConfig = new SearchConfig()
            {
                Path = basePath + "/TestComuni cation/OtherExCom ple/Com.txt",
                SearchExpression = @"bc<M",
                UseRegularExpression = false,
                CaseSensivity = true,
                AllowMultiMatch = true,
                SearchInDirectoryNames = true,
                SearchInFileNames = true,
                SearchInsideFiles = true,
            };
            searchVM.RunSearch();

            NullCharacterInsideMatch(searchVM.Matches[0].Items);
        }

        public static void NullCharacterInsideMatch(ObservableCollection<DirectoryMatch> items)
        {
            Assert.That(items.Where(i => i.MatchType != SharpTextSearcher.Model.MatchType.LINE).Count() == 0);

            var results = items.Select(a => a.NameFormatted);

            int initValue = MyTestsMethods.Count;

            MyTestsMethods.ContainsAndCount(results, "11\tabd<\0Match>def<\0/Match>asdasda<Match>bc<\0M</Match>atch>def<\0/Match>asdComasd");

            Assert.That(items.Select(i => i.Count).Sum() == MyTestsMethods.Count - initValue);
        }

        [Test]
        public void TestNullCharacterBeforeMatch()
        {
            Assert.That(basePath != "");

            MyTestsMethods.Initialize();

            searchVM.SearchConfig = new SearchConfig()
            {
                Path = basePath + "/TestComuni cation/OtherExCom ple/Com.txt",
                SearchExpression = @"/Mat",
                UseRegularExpression = false,
                CaseSensivity = true,
                AllowMultiMatch = true,
                SearchInDirectoryNames = true,
                SearchInFileNames = true,
                SearchInsideFiles = true,
            };
            searchVM.RunSearch();

            NullCharacterBeforeMatch(searchVM.Matches[0].Items);
        }

        public static void NullCharacterBeforeMatch(ObservableCollection<DirectoryMatch> items)
        {
            Assert.That(items.Where(i => i.MatchType != SharpTextSearcher.Model.MatchType.LINE).Count() == 0);

            var results = items.Select(a => a.NameFormatted);

            int initValue = MyTestsMethods.Count;

            MyTestsMethods.ContainsAndCount(results, "5\tabD<\0Match>def<<Match>\0/Mat</Match>ch>asdasd");
            MyTestsMethods.ContainsAndCount(results, "6\tabD<\0Match>deComf<<Match>\0/Mat</Match>ch>asdasd");
            MyTestsMethods.ContainsAndCount(results, "7\tabComc<\0Match>def<<Match>\0/Mat</Match>ch>asdasd");
            MyTestsMethods.ContainsAndCount(results, "8\tabd<\0Match>def<<Match>\0/Mat</Match>ch>aComsdasd");
            MyTestsMethods.ContainsAndCount(results, "9\tabDd<BComold>def<<Match>\0/Mat</Match>ch>asComdas");
            MyTestsMethods.ContainsAndCount(results, "11\tabd<\0Match>def<<Match>\0/Mat</Match>ch>asdasdabc<\0Match>def<<Match>\0/Mat</Match>ch>asdComasd");
            MyTestsMethods.ContainsAndCount(results, "12\tab<\0Match>def<\0<Match>/Mat</Match>ch>asdComasdabc<\0Match>def<<Match>\0/Mat</Match>ch>asdComasd");

            Assert.That(items.Select(i => i.Count).Sum() == MyTestsMethods.Count - initValue);
        }
    }
}