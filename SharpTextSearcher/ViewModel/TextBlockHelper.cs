using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using System.Xml;
using System.Drawing;
using System.Windows.Media;

namespace SharpTextSearcher.ViewModel
{
    // FROM https://stackoverflow.com/questions/5565885/how-to-bind-a-textblock-to-a-resource-containing-formatted-text
    // with some mods
    public static class TextBlockHelper
    {
        public static string GetFormattedText(DependencyObject obj)
        {
            return (string)obj.GetValue(FormattedTextProperty);
        }

        public static void SetFormattedText(DependencyObject obj, string value)
        {
            obj.SetValue(FormattedTextProperty, value);
        }

        public static readonly DependencyProperty FormattedTextProperty =
            DependencyProperty.RegisterAttached("FormattedText",
            typeof(string),
            typeof(TextBlockHelper),
            new UIPropertyMetadata("", FormattedTextChanged));

        static Inline Traverse(string value)
        {
            // Get the sections/inlines
            string[] sections = SplitIntoSections(value);

            // Check for grouping
            if (sections.Length.Equals(1))
            {
                string section = sections[0];
                string token; // E.g <Match>
                int tokenStart, tokenEnd; // Where the token/section starts and ends.

                // Check for token
                if (GetTokenInfo(section, out token, out tokenStart, out tokenEnd))
                {
                    // Get the content to further examination
                    string content = token.Length.Equals(tokenEnd - tokenStart) ?
                        null :
                        section.Substring(token.Length, section.Length - 1 - token.Length * 2);

                    switch (token)
                    {
                        case "<Match>":
                            return new Bold(Traverse(content)) { Background = new SolidColorBrush(Colors.LightGray) };
                        case "<Italic>":
                            return new Italic(Traverse(content));
                        case "<Underline>":
                            return new Underline(Traverse(content));
                        case "<LineBreak/>":
                            return new LineBreak();
                        default:
                            return new Run(section);
                    }
                }
                else return new Run(section);
            }
            else // Group together
            {
                Span span = new Span();

                foreach (string section in sections)
                    span.Inlines.Add(Traverse(section));

                return span;
            }
        }

        /// <summary>
        /// Examines the passed string and find the first token, where it begins and where it ends.
        /// </summary>
        /// <param name="value">The string to examine.</param>
        /// <param name="token">The found token.</param>
        /// <param name="startIndex">Where the token begins.</param>
        /// <param name="endIndex">Where the end-token ends.</param>
        /// <returns>True if a token was found.</returns>
        static bool GetTokenInfo(string value, out string token, out int startIndex, out int endIndex)
        {
            token = "<Match>";
            string tokenEnd = "</Match>";
            startIndex = -1;
            endIndex = -1;

            if (!value.Contains(token) || !value.Contains(tokenEnd))
            {
                return false;
            }
            
            startIndex = value.Split(token)[0].Length; // Since "value.IndexOf("aa<\0Match>cc")" is > 0
            int startTokenEndIndex = startIndex + token.Length - 1;

            // No token here
            if (startIndex < 0 || startTokenEndIndex < 0)
                return false;

            endIndex = value.Split(tokenEnd)[0].Length + tokenEnd.Length;

            return true;
        }

        /// <summary>
        /// Splits the string into sections of tokens and regular text.
        /// </summary>
        /// <param name="value">The string to split.</param>
        /// <returns>An array with the sections.</returns>
        static string[] SplitIntoSections(string value)
        {
            List<string> sections = new List<string>();

            while (!string.IsNullOrEmpty(value))
            {
                string token;
                int tokenStartIndex, tokenEndIndex;

                // Check if this is a token section
                if (GetTokenInfo(value, out token, out tokenStartIndex, out tokenEndIndex))
                {
                    // Add pretext if the token isn't from the start
                    if (tokenStartIndex > 0)
                        sections.Add(value.Substring(0, tokenStartIndex));

                    sections.Add(value.Substring(tokenStartIndex, tokenEndIndex - tokenStartIndex));
                    value = value.Substring(tokenEndIndex); // Trim away
                }
                else
                { // No tokens, just add the text
                    sections.Add(value);
                    value = null;
                }
            }

            return sections.ToArray();
        }

        private static void FormattedTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            string value = e.NewValue as string;

            TextBlock textBlock = sender as TextBlock;

            if (textBlock != null)
                textBlock.Inlines.Add(Traverse(value));
        }
    }
}
