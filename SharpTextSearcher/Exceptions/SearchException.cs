using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpTextSearcher.Exceptions
{
    internal class SearchException : Exception
    {
        public SearchException(string? name, int lineNumber, bool isLine) : base(MessageBuilder(name, lineNumber, isLine))
        {
        }

        public SearchException() : base()
        {
        }

        public SearchException(string? message) : base(message)
        {
        }

        public SearchException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        private static string MessageBuilder(string? name, int lineNumber, bool isLine)
        {
            if (isLine)
            {
                return $"Error while searching on file \"{name}\" at line {lineNumber}";
            }
            else
            {
                return $"Error while searching on \"{name}\" name";
            }
        }
    }
}
