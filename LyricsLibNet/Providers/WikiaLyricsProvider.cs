using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LyricsLibNet.Providers
{
    public class WikiaLyricsProvider : HttpLyricsProviderBase
    {
        public override string Name
        {
            get { return "wikia.com"; }
        }

        private string FixName(string name)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in name)
            {
                if (char.IsWhiteSpace(c))
                {
                    builder.Append('_');
                }
                else
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }

        public override string GetRequestUrl(string artist, string title)
        {
            return "http://lyrics.wikia.com/" + Uri.EscapeDataString(FixName(artist)) + ":" + Uri.EscapeDataString(FixName(title));
        }

        public override LyricsResult ParseResult(string requestArtist, string requestTrackTitle, string result)
        {
            string start = @"right.gif' alt='phone' width='16' height='17'/></a></div>";
            string end = @"<!--";
            string code = StringHelper.GetTextBetween(result, start, end);
            code = code.Replace("<br />", Environment.NewLine);
            Regex replacer = new Regex("&#(?<code>[0-9]+);");
            code = replacer.Replace(code, m =>
            {
                int htmlCode;
                if (int.TryParse(m.Groups["code"].Value, out htmlCode))
                {
                    return ((char)htmlCode).ToString();
                }
                return "";
            });
            code = code.Trim();
            return new LyricsResult(this.Name, requestArtist, requestTrackTitle, code);
        }
    }
}
