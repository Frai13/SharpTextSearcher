using SharpTextSearcher.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;
using static SharpTextSearcher.Model.MatchElement;

namespace SharpTextSearcher.Model
{
    // https://stackoverflow.com/questions/46651448/wpf-treeview-and-databinding-a-directory-tree
    public class DirectoryMatch : ObservableObject
    {
        public ObservableCollection<DirectoryMatch> Items { get; set; }
        public string Name { get; set; }
        private string _NameFormatted = "";
        public string NameFormatted
        {
            get { return _NameFormatted; }
            set { _NameFormatted = value; OnPropertyChanged(); }
        }
        public MatchType MatchType { get; set; } = MatchType.LINE;
        public bool Expand { get; set; }
        private int _Count = 0;
        public int Count
        {
            get
            {
                if (Items.Count == 0)
                {
                    return _Count;
                }
                else
                {
                    return Items.Select(i => i.Count).Sum() + _Count;
                }
            }
        }



        public DirectoryMatch()
        {
            Name = "";
            NameFormatted = "";
            Expand = Global.SearchConfig.ExpandDirectoryItems;
            Items = new ObservableCollection<DirectoryMatch>();
        }

        public DirectoryMatch(string searchPath)
        {
            // Remove last slash
            searchPath = searchPath.Last() == '/' || searchPath.Last() == '\\' ? searchPath.Remove(searchPath.Length - 1) : searchPath;

            Name = Path.GetFileName(searchPath);
            NameFormatted = Name;
            Expand = Global.SearchConfig.ExpandDirectoryItems;
            Items = new ObservableCollection<DirectoryMatch>();

            string searchExp = Global.SearchConfig.SearchExpression;

            bool PathIsDirectory = true;
            try
            {
                PathIsDirectory = IsDirectory(searchPath);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                throw ex;
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                throw ex;
            }

            MatchType = PathIsDirectory ? MatchType.DIRECTORY : MatchType.FILE;

            if (searchExp == "")
            {
                return;
            }

            List<MatchElement> matchElementList = new List<MatchElement>();

            // Search in Directories and Files
            List<string> directories = PathIsDirectory && Global.SearchConfig.SearchInDirectoryNames ? Directory.GetDirectories(searchPath, "*", SearchOption.AllDirectories).ToList() : new List<string>();
            List<string> files = PathIsDirectory && Global.SearchConfig.SearchInFileNames ? Directory.GetFiles(searchPath, "*", SearchOption.AllDirectories).ToList() : new List<string>();
            files = PathIsDirectory ? files : new List<string>() { searchPath};

            foreach (var pathMatch in directories.Concat(files))
            {
                List<string> folders = Path.GetRelativePath(searchPath, pathMatch).Split('\\').ToList();
                for (int i = 0; i < folders.Count; i++)
                {
                    // The previous path have the pathMatch and every folder before the match folder (match in index 1, I take 1 folder...) 
                    string previousPath = String.Join("", folders.Take(i + 1).Select(f => "/" + f));
                    string path = searchPath + previousPath;

                    if (!Global.SearchConfig.SearchInDirectoryNames && IsDirectory(path))
                    {
                        continue;
                    }

                    Search(ref matchElementList, path, folders[i]);
                }
            }

            // Search inside Files
            files = PathIsDirectory && Global.SearchConfig.SearchInsideFiles ? Directory.GetFiles(searchPath, "*", SearchOption.AllDirectories).ToList() : new List<string>();
            files = PathIsDirectory ? files : new List<string>() { searchPath };

            foreach (var file in files)
            {
                foreach (var line in File.ReadAllLines(file).Select((v, i) => new {v, i}))
                {
                    Search(ref matchElementList, file, line.v, line.i);
                }
            }

            string parentPath = Path.GetDirectoryName(searchPath) ?? "";
            if (PathIsDirectory)
            {
                matchElementList.ForEach(i => AddDirectory(parentPath, i));
            }
            else
            {
                matchElementList.ForEach(i => AddFile(parentPath, i));
            }
        }

        public void Search(ref List<MatchElement> matchElementList, string path, string input, int lineNumber = -1)
        {
            if (Global.SearchConfig.UseRegularExpression)
            {
                MatchCollection m = Regex.Matches(input, Global.SearchConfig.SearchExpression, Global.SearchConfig.CaseSensivity ? RegexOptions.None : RegexOptions.IgnoreCase);
                if (m.Count > 0)
                {
                    List<Match> matches = Global.SearchConfig.AllowMultiMatch ? m.ToList() : m.Take(1).ToList();
                    List<MatchPosition> matchesPos = new List<MatchPosition>();
                    matches.ForEach(a => matchesPos.Add(new MatchPosition(a)));
                    matchElementList.Add(new MatchElement(path, matchesPos, input, lineNumber + 1));
                }
            }
            else
            {
                List<MatchPosition> matches = new List<MatchPosition>();
                int length = Global.SearchConfig.SearchExpression.Length;
                int index = input.IndexOf(Global.SearchConfig.SearchExpression, 0, Global.SearchConfig.CaseSensivity ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
                while (index >= 0)
                {
                    matches.Add(new MatchPosition(index, length));
                    index = input.IndexOf(Global.SearchConfig.SearchExpression, index + length, Global.SearchConfig.CaseSensivity ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
                }
                if (matches.Count > 0)
                {
                    List<MatchPosition> matchesPos = Global.SearchConfig.AllowMultiMatch ? matches : matches.Take(1).ToList();
                    matchElementList.Add(new MatchElement(path, matchesPos, input, lineNumber + 1));
                }
            }
        }
        public bool IsDirectory(string path)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                return attr.HasFlag(FileAttributes.Directory);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                throw ex;
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                throw ex;
            }
        }

        public void AddDirectory(string parentPath, MatchElement element)
        {
            Application.Current?.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

            MatchType = MatchType.DIRECTORY;
            string[] relativePath = Path.GetRelativePath(parentPath, element.Path).Split('\\');
            Name = relativePath[0];
            Expand = Global.SearchConfig.ExpandDirectoryItems;

            if (relativePath.Count() != 1)
            {
                if (NameFormatted == "")
                {
                    NameFormatted = Name;
                }
                string Child = relativePath[1];
                if (IsDirectory(parentPath + @"\" + Name + @"\" + Child))
                {
                    if (Items.Where(s => s.Name == Child).Count() == 0)
                    {
                        Items.Add(new DirectoryMatch());
                        Items.Last().AddDirectory(parentPath + @"\" + Name, element);
                    }
                    else
                    {
                        Items.Where(s => s.Name == Child).First().AddDirectory(parentPath + @"\" + Name, element);
                    }
                }
                else
                {
                    if (Items.Where(s => s.Name == Child).Count() == 0)
                    {
                        Items.Add(new DirectoryMatch());
                        Items.Last().AddFile(parentPath + @"\" + Name, element);
                    }
                    else
                    {
                        Items.Where(s => s.Name == Child).First().AddFile(parentPath + @"\" + Name, element);
                    }
                }
            }
            else
            {
                if (element.IsMatch)
                {
                    NameFormatted = element.FormatMatch(Name);
                    _Count = element.MatchesPos.Count;
                }
            }
        }

        public void AddFile(string parentPath, MatchElement element)
        {
            Application.Current?.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

            MatchType = MatchType.FILE;
            string[] relativePath = Path.GetRelativePath(parentPath, element.Path).Split('\\');
            Name = relativePath[0];
            Expand = Global.SearchConfig.ExpandFileItems;

            if (element.IsLine)
            {
                if (NameFormatted == "")
                {
                    NameFormatted = Name;
                }
                Items.Add(new DirectoryMatch());
                Items.Last().AddLine(element);
            }
            else
            {
                if (element.IsMatch)
                {
                    NameFormatted = element.FormatMatch(Name);
                    _Count = element.MatchesPos.Count;
                }
            }
        }

        public void AddLine(MatchElement element)
        {
            Application.Current?.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

            Name = element.Line;
            MatchType = MatchType.LINE;

            NameFormatted = element.FormatMatch(element.Line);
            _Count = element.MatchesPos.Count;
        }
    }
}
