using CommunityToolkit.Mvvm.Input;
using SharpTextSearcher.Core;
using SharpTextSearcher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpTextSearcher.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public SearchViewModel SearchVM { get; set; }
        public ConfigViewModel ConfigVM { get; set; }
        public RelayCommand ConfigViewCommand { get; set; }

        #region PROPERTIES
        public bool ShowConfig { get; set; }

        private object? _mainView;
        public object? MainView
        {
            get { return _mainView; }
            set
            {
                _mainView = value;
                OnPropertyChanged();
            }
        }

        private object? _leftView;
        public object? LeftView
        {
            get { return _leftView; }
            set
            {
                _leftView = value;
                OnPropertyChanged();
            }
        }
        #endregion PROPERTIES

        public MainViewModel()
        {
            ShowConfig = false;

            SearchVM = new SearchViewModel();
            ConfigVM = new ConfigViewModel();
            ConfigViewCommand = new RelayCommand(delegate ()
            {
                if (ShowConfig)
                {
                    LeftView = ConfigVM;
                }
                else
                {
                    LeftView = null;
                }
            });

            MainView = SearchVM;
            LeftView = null;
        }
    }
}
