using Guppy.Extensions.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library
{
    public static class VHR
    {
        public static class Entities
        {
            public static readonly String RigidShipPart = "entity:ship-part:rigid-ship-part";
            public static readonly String Thruster = "entity:ship-part:thruster";
            public static readonly String Gun = "entity:ship-part:weapon:gun";
            public static readonly String Armor = "entity:ship-part:armor";
        }

        public static class MessageTypes
        {
            public static class Scene
            {
                public static readonly UInt32 Setup = "scene:setup".xxHash();
                public static readonly UInt32 Entity = "scene:entity".xxHash();
            }

            public static class World
            {
                public static readonly UInt32 UpdateSize = "world:update:size".xxHash();
            }

            public static class ShipPart
            {
                public static readonly UInt32 UpdateHealth = "ship-part:update:health".xxHash();
            }
            
            public static class Ship
            {
                public static readonly UInt32 UpdateTarget = "ship:update:target".xxHash();
                public static readonly UInt32 UpdateTargetRequest = "ship:update:target:request".xxHash();

                public static readonly UInt32 UpdateDirection = "ship:update:direction".xxHash();
                public static readonly UInt32 UpdateDirectionRequest = "ship:update:direction:request".xxHash();
                
                public static readonly UInt32 UpdateFiring = "ship:update:firing".xxHash();
                public static readonly UInt32 UpdateFiringRequest = "ship:update:firing:request".xxHash();
                
                public static readonly UInt32 UpdateBridge = "ship:update:bridge".xxHash();

                public static readonly UInt32 SpawnRequest = "ship:spawn:request".xxHash();

                public static readonly UInt32 SpawnAiRequest = "ship:spawn:ai:request".xxHash();

                public static readonly UInt32 SelfDestructRequest = "ship:self-destruct:request".xxHash();
                
                public static class TractorBeam
                {
                    public static readonly UInt32 Action = "ship:tractor-beam:action".xxHash();
                    public static readonly UInt32 ActionRequest = "ship:tractor-beam:action:request".xxHash();
                }
            }
        }
    }
}
