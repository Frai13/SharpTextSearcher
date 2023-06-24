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
    public class TestsRegexFilename
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

            BasicMatch(searchVM.Matches[0].Items);
        }

        public static void BasicMatch(ObservableCollection<DirectoryMatch> items)
        {
            Assert.That(items.Where(i => i.MatchType != SharpTextSearcher.Model.MatchType.LINE).Count() == 0);

            var results = items.Select(a => a.NameFormatted);

            int initValue = MyTestsMethods.Count;

            MyTestsMethods.ContainsAndCount(results, "1\tHell<Match>oCom</Match>thisiscomexample co<Match>mCom</Match>");
            MyTestsMethods.ContainsAndCount(results, "3\t<Match> Com</Match>com ");
            MyTestsMethods.ContainsAndCount(results, "4\tco<Match>mCom</Match> purposes");
            MyTestsMethods.ContainsAndCount(results, "6\tabD<\0Match>d<Match>eCom</Match>f<\0/Match>asdasd");
            MyTestsMethods.ContainsAndCount(results, "7\ta<Match>bCom</Match>c<\0Match>def<\0/Match>asdasd");
            MyTestsMethods.ContainsAndCount(results, "8\tabd<\0Match>def<\0/Match><Match>aCom</Match>sdasd");
            MyTestsMethods.ContainsAndCount(results, "9\tabDd<<Match>BCom</Match>old>def<\0/Match>a<Match>sCom</Match>das");
            MyTestsMethods.ContainsAndCount(results, "10\tdabD<\0Match>def</B<Match>oCom</Match>ld>asd<Match>aCom</Match>sd");
            MyTestsMethods.ContainsAndCount(results, "11\tabd<\0Match>def<\0/Match>asdasdabc<\0Match>def<\0/Match>as<Match>dCom</Match>asd");
            MyTestsMethods.ContainsAndCount(results, "12\tab<\0Match>def<\0/Match>as<Match>dCom</Match>asdabc<\0Match>def<\0/Match>as<Match>dCom</Match>asd");
            MyTestsMethods.ContainsAndCount(results, "14\tdadsasdcas<Match>cCom</Match>");
            MyTestsMethods.ContainsAndCount(results, "15\tComASdadsa<Match>dCom</Match>");
            MyTestsMethods.ContainsAndCount(results, "17\tdadsas<\0Match>dcas<Match>cCom</Match>");
            MyTestsMethods.ContainsAndCount(results, "18\tComASd<\0Match>adsa<Match>dCom</Match>");
            MyTestsMethods.ContainsAndCount(results, "19\taa<Match>\0Com</Match>\0bb");
            MyTestsMethods.ContainsAndCount(results, "21\t<Match>\0Com</Match>");

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

            MyTestsMethods.ContainsAndCount(results, "1\tHell<Match>oCom</Match>thisi<Match>scom</Match>example<Match> com</Match>Com");
            MyTestsMethods.ContainsAndCount(results, "2\tbut onlyfor<Match> com</Match> testing");
            MyTestsMethods.ContainsAndCount(results, "3\t<Match> Com</Match>com ");
            MyTestsMethods.ContainsAndCount(results, "4\tco<Match>mCom</Match> purposes");
            MyTestsMethods.ContainsAndCount(results, "6\tabD<\0Match>d<Match>eCom</Match>f<\0/Match>asdasd");
            MyTestsMethods.ContainsAndCount(results, "7\ta<Match>bCom</Match>c<\0Match>def<\0/Match>asdasd");
            MyTestsMethods.ContainsAndCount(results, "8\tabd<\0Match>def<\0/Match><Match>aCom</Match>sdasd");
            MyTestsMethods.ContainsAndCount(results, "9\tabDd<<Match>BCom</Match>old>def<\0/Match>a<Match>sCom</Match>das");
            MyTestsMethods.ContainsAndCount(results, "10\tdabD<\0Match>def</B<Match>oCom</Match>ld>asd<Match>aCom</Match>sd");
            MyTestsMethods.ContainsAndCount(results, "11\tabd<\0Match>def<\0/Match>asdasda<Match>bc<\0M</Match>atch>def<\0/Match>as<Match>dCom</Match>asd");
            MyTestsMethods.ContainsAndCount(results, "12\tab<\0Match>def<\0/Match>as<Match>dCom</Match>asdabc<\0Match>def<\0/Match>as<Match>dCom</Match>asd");
            MyTestsMethods.ContainsAndCount(results, "14\tdadsasdcas<Match>cCom</Match>");
            MyTestsMethods.ContainsAndCount(results, "15\tComASdadsa<Match>dCom</Match>");
            MyTestsMethods.ContainsAndCount(results, "17\tdadsas<\0Match>dcas<Match>cCom</Match>");
            MyTestsMethods.ContainsAndCount(results, "18\tComASd<\0Match>adsa<Match>dCom</Match>");
            MyTestsMethods.ContainsAndCount(results, "19\taa<Match>\0Com</Match>\0bb");
            MyTestsMethods.ContainsAndCount(results, "21\t<Match>\0Com</Match>");

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

            MyTestsMethods.ContainsAndCount(results, "1\tHell<Match>oCom</Match>thisiscomexample comCom");
            MyTestsMethods.ContainsAndCount(results, "2\tbut onlyfor<Match> com</Match> testing");
            MyTestsMethods.ContainsAndCount(results, "3\t<Match> Com</Match>com ");
            MyTestsMethods.ContainsAndCount(results, "4\tco<Match>mCom</Match> purposes");
            MyTestsMethods.ContainsAndCount(results, "6\tabD<\0Match>d<Match>eCom</Match>f<\0/Match>asdasd");
            MyTestsMethods.ContainsAndCount(results, "7\ta<Match>bCom</Match>c<\0Match>def<\0/Match>asdasd");
            MyTestsMethods.ContainsAndCount(results, "8\tabd<\0Match>def<\0/Match><Match>aCom</Match>sdasd");
            MyTestsMethods.ContainsAndCount(results, "9\tabDd<<Match>BCom</Match>old>def<\0/Match>asComdas");
            MyTestsMethods.ContainsAndCount(results, "10\tdabD<\0Match>def</B<Match>oCom</Match>ld>asdaComsd");
            MyTestsMethods.ContainsAndCount(results, "11\tabd<\0Match>def<\0/Match>asdasda<Match>bc<\0M</Match>atch>def<\0/Match>asdComasd");
            MyTestsMethods.ContainsAndCount(results, "12\tab<\0Match>def<\0/Match>as<Match>dCom</Match>asdabc<\0Match>def<\0/Match>asdComasd");
            MyTestsMethods.ContainsAndCount(results, "14\tdadsasdcas<Match>cCom</Match>");
            MyTestsMethods.ContainsAndCount(results, "15\tComASdadsa<Match>dCom</Match>");
            MyTestsMethods.ContainsAndCount(results, "17\tdadsas<\0Match>dcas<Match>cCom</Match>");
            MyTestsMethods.ContainsAndCount(results, "18\tComASd<\0Match>adsa<Match>dCom</Match>");
            MyTestsMethods.ContainsAndCount(results, "19\taa<Match>\0Com</Match>\0bb");
            MyTestsMethods.ContainsAndCount(results, "21\t<Match>\0Com</Match>");

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
                UseRegularExpression = true,
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
                UseRegularExpression = true,
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