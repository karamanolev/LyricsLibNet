using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace LyricsLibNet
{
    public class LyricsFinder
    {
        private static Type[] EmptyTypes = new Type[0];

        private int requestsPending;
        private bool started;
        private List<ILyricsProvider> providers;
        private List<LyricsResult> results;

        public string Artist { get; private set; }
        public string TrackTitle { get; private set; }
        public LyricsResult[] Results
        {
            get
            {
                lock (this.results)
                {
                    return this.results.ToArray();
                }
            }
        }

        public LyricsFinder(string artist, string trackTitle)
        {
            this.results = new List<LyricsResult>();
            this.providers = new List<ILyricsProvider>();
            this.AddProvidersForAssembly(typeof(LyricsFinder).Assembly);

            this.Artist = artist;
            this.TrackTitle = trackTitle;
        }

        public void ClearProviders()
        {
            if (this.started)
            {
                throw new InvalidOperationException("Already started.");
            }

            this.providers.Clear();
        }

        public void AddProvidersForAssembly(Assembly assembly)
        {
            Type targetInterface = typeof(ILyricsProvider);
            foreach (Type t in assembly.GetTypes().Where(t => targetInterface.IsAssignableFrom(t)))
            {
                if (t.IsInterface) continue;
                if (t.IsAbstract) continue;
                ConstructorInfo constructor = t.GetConstructor(EmptyTypes);
                ILyricsProvider provider = (ILyricsProvider)constructor.Invoke(null);
                this.AddProvider(provider);
            }
        }

        public void AddProvider(ILyricsProvider provider)
        {
            if (this.started)
            {
                throw new InvalidOperationException("Already started.");
            }

            this.providers.Add(provider);
        }

        public void Start()
        {
            if (this.started)
            {
                throw new InvalidOperationException("Already started.");
            }

            if (this.providers.Count == 0)
            {
                throw new InvalidOperationException("No providers registered.");
            }

            this.started = true;

            this.requestsPending = this.providers.Count;

            foreach (ILyricsProvider provider in this.providers)
            {
                provider.Query(this.Artist, this.TrackTitle, result =>
                {
                    bool isLast = Interlocked.Decrement(ref this.requestsPending) == 0;
                    this.OnResultAvailable(isLast, result);
                    if (isLast)
                    {
                        this.OnQueryCompleted();
                    }
                }, exception =>
                {
                    bool isLast = Interlocked.Decrement(ref this.requestsPending) == 0;
                    this.OnProviderError(isLast, provider.Name, exception);
                    if (isLast)
                    {
                        this.OnQueryCompleted();
                    }
                });
            }
        }

        public event EventHandler<ResultAvailableEventArgs> ResultAvailable;
        private void OnResultAvailable(bool isLast, LyricsResult lyricsResult)
        {
            lock (this.results)
            {
                this.results.Add(lyricsResult);
            }

            if (this.ResultAvailable != null)
            {
                this.ResultAvailable(this, new ResultAvailableEventArgs(isLast, lyricsResult));
            }
        }

        public event EventHandler QueryCompleted;
        private void OnQueryCompleted()
        {
            if (this.QueryCompleted != null)
            {
                this.QueryCompleted(this, EventArgs.Empty);
            }
        }

        public event EventHandler<ProviderErrorEventArgs> ProviderError;
        private void OnProviderError(bool isLast, string providerName, Exception exception)
        {
            if (this.ProviderError != null)
            {
                this.ProviderError(this, new ProviderErrorEventArgs(isLast, providerName, exception));
            }
        }
    }
}
