using System;
using System.Collections.Generic;
using System.Text;

namespace MobileStreamTesting.UDPServer
{
    public enum ServerPackets
    {
        ConnectionOk = 1, ActionStatusUpdate = 2, StringMessage = 3, Image = 4, VideoFrame = 5, Audio = 6,
    }

    public enum ClientPackets
    {
        ThankYou = 1, ActionStatusUpdate = 2, StringMessage = 3, Image = 4, VideoFrame = 5, Audio = 6,
    }
}
