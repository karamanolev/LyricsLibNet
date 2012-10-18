using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyricsLibNet.Providers
{
    class WikiaLyricsProvider : HttpLyricsProviderBase
    {
        public override string Name
        {
            get { return "wikia.com"; }
        }

        public override string GetRequestUrl(string artist, string title)
        {
            throw new NotImplementedException();
        }

        public override LyricsResult ParseResult(string requestArtist, string requestTrackTitle, string result)
        {
            throw new NotImplementedException();
        }
    }
}
