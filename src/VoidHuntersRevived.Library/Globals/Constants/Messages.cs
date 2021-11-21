using Guppy.Extensions.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Globals.Constants
{
    public static class Messages
    {
        public static class WorldObject
        {
            public static readonly UInt32 WorldInfoPing = "vhr:world-object:position-ping".xxHash();
        }

        public static class Chain
        {
            public static readonly UInt32 ShipPartAttached = "vhr:chain:ship-part:attached".xxHash();
        }

        public static class Ship
        {
            public static readonly UInt32 PlayerChanged = "vhr:ship:player:changed".xxHash();
            public static readonly UInt32 TargetChanged = "vhr:ship:target:changed".xxHash();
            public static readonly UInt32 DirectionChanged = "vhr:ship:direction:changed".xxHash();
            public static readonly UInt32 TractorBeamAction = "vhr:ship:tractor-beam:action".xxHash();
        }

        public static class UserPlayer
        {
            public static readonly UInt32 RequestDirectionChanged = "vhr:user-player:request:direction:changed".xxHash();
            public static readonly UInt32 RequestTractorBeamAction = "vhr:user-player:request:tractor-beam:action".xxHash();
            public static readonly UInt32 RequestTargetChangedAction = "vhr:user-player:request:target:changed".xxHash();
        }
    }
}
