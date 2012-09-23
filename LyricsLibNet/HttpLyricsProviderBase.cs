using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace LyricsLibNet
{
    public abstract class HttpLyricsProviderBase : ILyricsProvider
    {
        class State
        {
            public string Artist { get; set; }
            public string Title { get; set; }
            public HttpWebRequest Request { get; set; }
            public Action<LyricsResult> SuccessCallback { get; set; }
            public Action<Exception> ErrorCallback { get; set; }
        }

        public abstract string Name { get; }

        public abstract string GetRequestUrl(string artist, string title);
        public abstract LyricsResult ParseResult(string requestArtist, string requestTrackTitle, string result);

        public void Query(string artist, string title, Action<LyricsResult> resultCallback, Action<Exception> errorCallback)
        {
            try
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp(this.GetRequestUrl(artist, title));
                request.BeginGetResponse(this.ResponseCallback, new State()
                {
                    Artist = artist,
                    Title = title,
                    Request = request,
                    SuccessCallback = resultCallback,
                    ErrorCallback = errorCallback
                });
            }
            catch (Exception ex)
            {
                errorCallback(ex);
            }
        }

        private void ResponseCallback(IAsyncResult result)
        {
            State state = (State)result.AsyncState;

            try
            {
                HttpWebResponse response = (HttpWebResponse)state.Request.EndGetResponse(result);
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        string resultString = reader.ReadToEnd();
                        state.SuccessCallback(this.ParseResult(state.Artist, state.Title, resultString));
                    }
                }
            }
            catch (Exception ex)
            {
                state.ErrorCallback(ex);
            }
        }
    }
}