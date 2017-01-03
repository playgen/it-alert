﻿using PlayGen.Photon.Messaging;

namespace PlayGen.Photon.Messages.Room
{
    public  abstract class RoomMessage : Message
    {
        public override int Channel => (int)Channels.Room;
    }
}
