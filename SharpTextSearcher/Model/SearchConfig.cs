using SharpTextSearcher.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTextSearcher.Model
{
    public class SearchConfig : ObservableObject
    {
        #region SEARCH VIEW
        private string _Path = "";
        public string Path
        {
            get { return _Path; }
            set { _Path = value; OnPropertyChanged(); }
        }

        private string _SearchExpression = "";
        public string SearchExpression
        {
            get { return _SearchExpression; }
            set { _SearchExpression = value; OnPropertyChanged(); }
        }
        #endregion SEARCH VIEW

        #region CONFIG VIEW
        public string? AppVersion
        {
            get { return "v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString(3); }
        }
        private bool _UseRegularExpression = true;
        public bool UseRegularExpression
        {
            get { return _UseRegularExpression; }
            set { _UseRegularExpression = value; OnPropertyChanged(); }
        }

        private bool _CaseSensivity = true;
        public bool CaseSensivity
        {
            get { return _CaseSensivity; }
            set { _CaseSensivity = value; OnPropertyChanged(); }
        }

        private bool _AllowMultiMatch = true;
        public bool AllowMultiMatch
        {
            get { return _AllowMultiMatch; }
            set { _AllowMultiMatch = value; OnPropertyChanged(); }
        }

        private bool _SearchInDirectoryNames = true;
        public bool SearchInDirectoryNames
        {
            get { return _SearchInDirectoryNames; }
            set { _SearchInDirectoryNames = value; OnPropertyChanged(); }
        }

        private bool _SearchInFileNames = true;
        public bool SearchInFileNames
        {
            get { return _SearchInFileNames; }
            set { _SearchInFileNames = value; OnPropertyChanged(); }
        }

        private bool _SearchInsideFiles = true;
        public bool SearchInsideFiles
        {
            get { return _SearchInsideFiles; }
            set { _SearchInsideFiles = value; OnPropertyChanged(); }
        }

        private bool _ExpandDirectoryItems = true;
        public bool ExpandDirectoryItems
        {
            get { return _ExpandDirectoryItems; }
            set { _ExpandDirectoryItems = value; OnPropertyChanged(); }
        }

        private bool _ExpandFileItems = true;
        public bool ExpandFileItems
        {
            get { return _ExpandFileItems; }
            set { _ExpandFileItems = value; OnPropertyChanged(); }
        }
        #endregion CONFIG VIEW
    }
}
