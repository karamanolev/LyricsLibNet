using System;
using System.Linq;

namespace LyricsLibNet
{
    public class ProviderErrorEventArgs : EventArgs
    {
        public bool IsLast { get; private set; }
        public string ProviderName { get; private set; }
        public Exception Exception { get; private set; }

        public ProviderErrorEventArgs(bool isLast, string providerName, Exception exception)
        {
            this.IsLast = isLast;
            this.ProviderName = providerName;
            this.Exception = exception;
        }
    }
}
