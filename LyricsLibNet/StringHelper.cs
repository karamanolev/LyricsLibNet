using System;
using System.Linq;

namespace LyricsLibNet
{
    static class StringHelper
    {
        public static string GetTextBetween(string source, string start, string end)
        {
            int startIndex = source.IndexOf(start);
            int endIndex = source.IndexOf(end, startIndex);

            if (startIndex == -1 || endIndex == -1)
            {
                throw new InvalidOperationException("Start or end text not found.");
            }

            return source.Substring(startIndex + start.Length, endIndex - startIndex - start.Length);
        }
    }
}
