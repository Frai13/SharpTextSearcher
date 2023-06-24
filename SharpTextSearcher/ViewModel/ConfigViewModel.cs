using CommunityToolkit.Mvvm.Input;
using SharpTextSearcher.Core;
using SharpTextSearcher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SharpTextSearcher.ViewModel
{
    public class ConfigViewModel : ObservableObject
    {
        public RelayCommand CheckUpdatesCommand { get; set; }
        public SearchConfig SearchConfig
        {
            get { return Global.SearchConfig; }
            set { Global.SearchConfig = value; OnPropertyChanged(); }
        }

        public ConfigViewModel()
        {
            CheckUpdatesCommand = new RelayCommand(delegate () {
                UpdateManager.CheckForUpdates(true); });
        }
    }
}
