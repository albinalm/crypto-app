using System;

namespace CryptoGUIAvalonia.GUI
{
    public static class DataShare
    {
    }

    public class DataShareInstance
    {
        private Boolean _hasClosed;

        public Boolean HasClosed
        {
            get { return _hasClosed; }
            set
            {
                _hasClosed = value;
                // trigger event (you could even compare the new value to
                // the old one and trigger it when the value really changed)
            }
        }
    }
}