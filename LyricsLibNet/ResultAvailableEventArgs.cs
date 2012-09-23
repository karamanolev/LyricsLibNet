using System;
using System.Linq;

namespace LyricsLibNet
{
    public class ResultAvailableEventArgs : EventArgs
    {
        public bool IsLast { get; private set; }
        public LyricsResult Result { get; private set; }

        public ResultAvailableEventArgs(bool isLast, LyricsResult result)
        {
            this.IsLast = isLast;
            this.Result = result;
        }
    }
}
