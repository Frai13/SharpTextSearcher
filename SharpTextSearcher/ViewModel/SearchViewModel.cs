using CommunityToolkit.Mvvm.Input;
using SharpTextSearcher.Core;
using SharpTextSearcher.Model;
using SharpTextSearcher.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace SharpTextSearcher.ViewModel
{
    public class SearchViewModel : ObservableObject
    {
        private Thread tSearch;

        #region COMMANDS
        public RelayCommand SearchCommand { get; set; }
        #endregion COMMANDS

        #region PROPS
        private ObservableCollection<DirectoryMatch> _Matches = new ObservableCollection<DirectoryMatch>();
        public ObservableCollection<DirectoryMatch> Matches
        {
            get { return _Matches; }
            set { _Matches = value; OnPropertyChanged(); }
        }

        public SearchConfig SearchConfig
        {
            get { return Global.SearchConfig; }
            set { Global.SearchConfig = value; OnPropertyChanged(); }
        }

        private bool _IsSearching;
        public bool IsSearching
        {
            get { return _IsSearching; }
            set
            {
                _IsSearching = value;
                OnPropertyChanged();
            }
        }

        public bool IsNotSearching
        {
            get { return !_IsSearching; }
        }

        #endregion PROPS

        public SearchViewModel()
        {
            SearchCommand = new RelayCommand(delegate () { Search(); });
            tSearch = new Thread(RunSearchBackground);

            IsSearching = false;

            SearchConfig = new SearchConfig();
        }

        public void Search()
        {
            tSearch = new Thread(RunSearchBackground);
            tSearch.Start();
        }

        public void RunSearch()
        {
            Matches = new ObservableCollection<DirectoryMatch>() { new DirectoryMatch(SearchConfig.Path) };
        }

        public void RunSearchBackground()
        {
            Matches = new ObservableCollection<DirectoryMatch>();

            Application.Current?.Dispatcher.Invoke((Action)(() =>
            {
                IsSearching = true;

                try
                {
                    Matches.Add(new DirectoryMatch(SearchConfig.Path));
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    IsSearching = false;
                    MessageBoxWindow mbw = new MessageBoxWindow();
                    mbw.DataContext = new MessageBoxViewModel("Error", "Error: file or directory does not exists", "OK");
                    mbw.ShowDialog();
                }
                catch (System.IO.DirectoryNotFoundException ex)
                {
                    IsSearching = false;
                    MessageBoxWindow mbw = new MessageBoxWindow();
                    mbw.DataContext = new MessageBoxViewModel("Error", "Error: file or directory does not exists", "OK");
                    mbw.ShowDialog();
                }
                catch (Exceptions.SearchException ex)
                {
                    IsSearching = false;
                    MessageBoxWindow mbw = new MessageBoxWindow();
                    mbw.DataContext = new MessageBoxViewModel("Error", ex.Message, "OK");
                    mbw.ShowDialog();
                }
            }));

            IsSearching = false;
        }
    }
}
