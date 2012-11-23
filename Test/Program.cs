using System;
using System.Linq;
using System.Threading;
using LyricsLibNet;
using LyricsLibNet.Providers;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            AutoResetEvent autoEvent = new AutoResetEvent(false);

            LyricsFinder finder = new LyricsFinder("Supertramp", "It Doesn't Matter");
            finder.ClearProviders();
            finder.AddProvider(new WikiaLyricsProvider());
            finder.ResultAvailable += (sender, e) =>
            {
                Console.WriteLine(e.Result.Artist + " " + e.Result.TrackTitle + " : " + e.Result.Text.Substring(0, 40).Replace("\r\n", ""));
            };
            finder.ProviderError += (sender, e) =>
            {
                Console.WriteLine(e.ProviderName + ": " + e.Exception.Message);
            };
            finder.QueryCompleted += (sender, e) => autoEvent.Set();
            finder.Start();

            autoEvent.WaitOne();
        }
    }
}
