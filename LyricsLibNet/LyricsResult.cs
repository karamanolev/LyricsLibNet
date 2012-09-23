using System;
using System.Collections.Generic;
using System.Linq;

namespace LyricsLibNet
{
    public class LyricsResult
    {
        public string ProviderName { get; set; }
        public string Artist { get; set; }
        public string TrackTitle { get; set; }
        public string Text { get; set; }
        public Dictionary<string, string> AdditionalFields { get; private set; }

        public LyricsResult(string providerName, string artist, string trackTitle, string text)
        {
            this.ProviderName = providerName;
            this.Artist = artist;
            this.TrackTitle = trackTitle;
            this.Text = text;
            this.AdditionalFields = new Dictionary<string, string>();
        }
    }
}
