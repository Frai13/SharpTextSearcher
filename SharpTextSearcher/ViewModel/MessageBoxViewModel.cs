using SharpTextSearcher.Core;
using SharpTextSearcher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.Input;

namespace SharpTextSearcher.ViewModel
{
    public class MessageBoxViewModel : ObservableObject
    {
        public enum MyDialogResult { Left, Center, Right };

        #region COMMANDS
        public RelayCommand<Window>? LeftCommand { get; set; }
        public RelayCommand<Window>? CenterCommand { get; set; }
        public RelayCommand<Window>? RightCommand { get; set; }
        #endregion COMMANDS

        #region PROPS
        private string _Title = "Default Title";
        public string Title
        {
            get { return _Title; }
            set { _Title = value; OnPropertyChanged(); }
        }
        private string _Message = "Default Message";
        public string Message
        {
            get { return _Message; }
            set { _Message = value; OnPropertyChanged(); }
        }
        private string _LeftText = "";
        public string LeftText
        {
            get { return _LeftText; }
            set { _LeftText = value; OnPropertyChanged(); }
        }
        private string _CenterText = "";
        public string CenterText
        {
            get { return _CenterText; }
            set { _CenterText = value; OnPropertyChanged(); }
        }
        private string _RightText = "";
        public string RightText
        {
            get { return _RightText; }
            set { _RightText = value; OnPropertyChanged(); }
        }
        private int _ButtonsNumber = 1;
        public int ButtonsNumber
        {
            get { return _ButtonsNumber; }
            set { _ButtonsNumber = value; OnPropertyChanged(); }
        }
        private Visibility _VisibilityCenterButton = Visibility.Visible;
        public Visibility VisibilityCenterButton
        {
            get { return _VisibilityCenterButton; }
            set { _VisibilityCenterButton = value; OnPropertyChanged(); }
        }
        private Visibility _VisibilityRightButton = Visibility.Visible;
        public Visibility VisibilityRightButton
        {
            get { return _VisibilityRightButton; }
            set { _VisibilityRightButton = value; OnPropertyChanged(); }
        }
        private MyDialogResult _DialogResult;
        public MyDialogResult DialogResult
        {
            get { return _DialogResult; }
            set { _DialogResult = value; }
        }
        private GridLength _LeftPanelWidth;
        public GridLength LeftPanelWidth
        {
            get { return _LeftPanelWidth; }
            set { _LeftPanelWidth = value; }
        }
        private GridLength _CenterPanelWidth;
        public GridLength CenterPanelWidth
        {
            get { return _CenterPanelWidth; }
            set { _CenterPanelWidth = value; }
        }
        private GridLength _RightPanelWidth;
        public GridLength RightPanelWidth
        {
            get { return _RightPanelWidth; }
            set { _RightPanelWidth = value; }
        }
        #endregion PROPS

        public MessageBoxViewModel()
        {
        }

        public MessageBoxViewModel(string title, string message, string leftText, string centerText, string rightText)
        {
            ButtonsNumber = 3;

            LeftText = leftText;
            CenterText = centerText;
            RightText = rightText;
            Initialize(title, message);
        }

        public MessageBoxViewModel(string title, string message, string leftText, string centerText)
        {
            ButtonsNumber = 2;

            LeftText = leftText;
            CenterText = centerText;
            Initialize(title, message);
        }

        public MessageBoxViewModel(string title, string message, string leftText)
        {
            ButtonsNumber = 1;

            LeftText = leftText;
            Initialize(title, message);
        }

        private void Initialize(string title, string message)
        {
            Title = title;
            Message = message;

            LeftCommand = new RelayCommand<Window>(delegate (Window? window)
            {
                DialogResult = MyDialogResult.Left;
                this.CloseWindow(window);
            });

            CenterCommand = new RelayCommand<Window>(delegate (Window? window)
            {
                DialogResult = MyDialogResult.Center;
                this.CloseWindow(window);
            });

            RightCommand = new RelayCommand<Window>(delegate (Window? window)
            {
                DialogResult = MyDialogResult.Right;
                this.CloseWindow(window);
            });

            if (ButtonsNumber == 2)
            {
                LeftPanelWidth = new GridLength(1, GridUnitType.Star);
                CenterPanelWidth = new GridLength(1, GridUnitType.Star);
                RightPanelWidth = new GridLength(1, GridUnitType.Auto);

                VisibilityCenterButton = Visibility.Visible;
                VisibilityRightButton = Visibility.Collapsed;
            }
            else if (ButtonsNumber == 3)
            {
                LeftPanelWidth = new GridLength(1, GridUnitType.Star);
                CenterPanelWidth = new GridLength(1, GridUnitType.Star);
                RightPanelWidth = new GridLength(1, GridUnitType.Star);

                VisibilityCenterButton = Visibility.Visible;
                VisibilityRightButton = Visibility.Visible;
            }
            else
            {
                LeftPanelWidth = new GridLength(1, GridUnitType.Star);
                CenterPanelWidth = new GridLength(1, GridUnitType.Auto);
                RightPanelWidth = new GridLength(1, GridUnitType.Auto);

                VisibilityCenterButton = Visibility.Collapsed;
                VisibilityRightButton = Visibility.Collapsed;
            }
        }

        private void CloseWindow(Window? window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
    }
}
