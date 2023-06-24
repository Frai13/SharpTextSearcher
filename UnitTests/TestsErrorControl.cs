using SharpTextSearcher.Core;
using SharpTextSearcher.Model;
using SharpTextSearcher.ViewModel;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Windows;
using System.Reflection;
using UnitTests.Utils;

namespace UnitTests
{
    public class TestsErrorControl
    {
        private SearchViewModel searchVM = new SearchViewModel();
        private string basePath = Common.GetBasePath() ?? "";

        [SetUp]
        public void Setup()
        {
        }

        public void TestInitialization()
        {
            Assert.That(basePath != "");

            searchVM.SearchConfig = new SearchConfig()
            {
                Path = basePath + "/NoExists",
            };
        }

        [Test]
        public void TestFileNoExists()
        {
            searchVM.SearchConfig.Path = basePath + "/NoExists";
            Assert.Throws<System.IO.FileNotFoundException>(() => searchVM.RunSearch());
        }

        [Test]
        public void TestDirNoExists()
        {
            searchVM.SearchConfig.Path = basePath + "/NoExists/NoExists";
            Assert.Throws<System.IO.DirectoryNotFoundException>(() => searchVM.RunSearch());
        }
    }
}