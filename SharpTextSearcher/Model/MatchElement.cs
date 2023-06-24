using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpTextSearcher.Model
{
    public class MatchPosition
    {
        public int Index { get; set; } = -1;
        public int Length { get; set; } = -1;

        public MatchPosition(int index, int length)
        {
            Index = index;
            Length = length;
        }

        public MatchPosition(Match m)
        {
            Index = m.Index;
            Length = m.Length;
        }
    }

    public class MatchElement
    {
        public string Path { get; set; } = "";
        public bool IsMatch { get; set; } = false;
        public bool IsLine { get; set; } = false;
        public List<MatchPosition> MatchesPos { get; set; } = new List<MatchPosition>();
        public string Line { get; set; } = "";
        public int LineNumber { get; set; } = -1;

        public MatchElement(string path, List<MatchPosition> matchesPos, string line, int lineNumber)
        {
            Path = path;
            IsMatch = matchesPos.Count > 0;
            MatchesPos = matchesPos;

            if (lineNumber > 0)
            {
                IsLine = true;
                Line = line;
                LineNumber = lineNumber;
            }
        }

        public string FormatMatch(string name)
        {
            try
            {
                var aux = Replace(name, "<(/?Match)>", "<\0$1>");
                string result = aux.Item1;
                List<int> indexes_subs = aux.Item2;

                // Easier backward
                foreach (var m in MatchesPos.OrderByDescending(m => m.Index))
                {
                    int offsetIndex = indexes_subs.Where(a => m.Index > a).Count();
                    int newIndex = m.Index + offsetIndex;

                    int offsetLength = indexes_subs.Where(a => m.Index + m.Length > a).Count() - offsetIndex;
                    int newLength = m.Length + offsetLength;

                    result = result.Insert(newIndex + newLength, "</Match>").Insert(newIndex, "<Match>");
                }

                return IsLine ? $"{LineNumber}\t" + result : result;
            }
            catch (Exception e)
            {
                throw new Exceptions.SearchException(Path, LineNumber, IsLine);
            }
        }

        private (string, List<int>) Replace(string input, string pattern, string replacement)
        {
            string original_input = input;

            string mod = Regex.Replace(input, pattern, replacement);

            List<int> indexes = new List<int>();
            int acc = 0;
            for (int i = 0; true; i++)
            {
                string original_subs = new(original_input.Skip(acc).ToArray());
                string mod_subs = new(mod.Skip(acc + i * 1).ToArray());

                int last_index = original_subs.Zip(mod_subs, (c1, c2) => c1 == c2).TakeWhile(b => b).Count();
                if (original_subs == mod_subs)
                {
                    break;
                }
                indexes.Add(acc + last_index);
                acc += last_index;
            }

            return (mod, indexes);
        }
    }
}
