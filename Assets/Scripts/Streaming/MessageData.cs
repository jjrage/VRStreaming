using System;
using System.Collections.Generic;
using System.Text;

namespace StreamingLibrary
{
    public class MessageData
    {
        public string Sender { get; set; }
        public string TextMessage { get; set; }
        public byte[] ByteMessage { get; set; }
    }
}
