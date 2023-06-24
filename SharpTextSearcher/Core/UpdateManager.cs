using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoUpdaterDotNET;
using SharpTextSearcher.View;
using SharpTextSearcher.ViewModel;

namespace SharpTextSearcher.Core
{
    public static class UpdateManager
    {
        private static bool isAlreadySubscribed = false;
        private static bool ForceCheck;
        public enum MyDialogResult { OkYes, RemindLater, No };

        public static MyDialogResult ShowMessage(string title, string text, int ButtonsNumber)
        {
            MessageBoxWindow mbw = new MessageBoxWindow();
            if (ButtonsNumber == 3)
            {
                mbw.DataContext = new MessageBoxViewModel(title, text, "Yes", "Remind later", "No");
            }
            else
            {
                mbw.DataContext = new MessageBoxViewModel(title, text, "OK");
            }
            mbw.ShowDialog();

            return (MyDialogResult)((MessageBoxViewModel)mbw.DataContext).DialogResult;
        }

        public static void CheckForUpdates(bool forceCheck)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(7))
            {
                ShowMessage("Update Warning", "This OS does not support updates", 1);
                return;
            }

            if (!isAlreadySubscribed)
            {
                AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
                isAlreadySubscribed = true;
            }

            string? appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            AutoUpdater.ReportErrors = true;
            AutoUpdater.DownloadPath = appPath;
            AutoUpdater.ExecutablePath = "setup.exe";

            ForceCheck = forceCheck;
            if (ForceCheck)
            {
                AutoUpdater.Mandatory = true;
            }

            AutoUpdater.Start("https://raw.githubusercontent.com/Frai13/SharpTextSearcher/master/AutoUpdater/autoupdater.xml");
        }

        private static void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(7)) return;

            if (args.Error == null)
            {
                if (args.IsUpdateAvailable)
                {
                    MyDialogResult dialogResult;
                    if (args.Mandatory.Value)
                    {
                        dialogResult = ShowMessage("New Update", $@"There is new version {args.CurrentVersion} available. You are using version {args.InstalledVersion.ToString(3)} " +
                            "This is required update. Press Ok to begin updating the application.", 1);
                    }
                    else
                    {
                        dialogResult = ShowMessage("New Update", $@"There is new version {args.CurrentVersion} available. You are using version {args.InstalledVersion.ToString(3)} " +
                            "Do you want to update the application now?", 3);
                    }

                    if (dialogResult == MyDialogResult.OkYes)
                    {
                        try
                        {
                            if (AutoUpdater.DownloadUpdate(args))
                            {
                                Application.Exit();
                            }
                        }
                        catch (Exception exception)
                        {
                            ShowMessage("Update Warning", $"Error on download: {exception.Message}", 1);
                        }
                    }
                    else if (dialogResult == MyDialogResult.RemindLater)
                    {
                        DateTime nextUpdate= DateTime.Now + new TimeSpan(0, 1, 0);
                        AutoUpdater.PersistenceProvider.SetRemindLater(nextUpdate);
                    }
                    else
                    {
                        AutoUpdater.PersistenceProvider.SetSkippedVersion(Version.Parse(args.CurrentVersion));
                    }
                }
                else if (ForceCheck)
                {
                    ShowMessage("Update Manage", $@"There is no new version available. You are using version {args.InstalledVersion.ToString(3)}", 1);
                }
            }
            else
            {
                if (args.Error is WebException)
                {
                    ShowMessage("Update Warning", @"There is a problem reaching update server. Please check your internet connection and try again later.", 1);
                }
                else
                {
                    ShowMessage("Update Warning", args.Error.Message, 1);
                }
            }
        }
    }
}
