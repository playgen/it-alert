using System;

namespace PlayGen.ITAlert.Network.Client
{
    [Flags]
    public enum ListRoomsFilters
    {
        None    = 1 << 0,
        Open    = 1 << 1,
        Closed  = 1 << 2,
        Visible = 1 << 3,
        Hidden  = 1 << 4,
    }
}