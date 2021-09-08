using Guppy.Contexts;
using Guppy.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.LayerGroups;
using Guppy.Network.Contexts;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Components.Entities.WorldObjects;

namespace VoidHuntersRevived.Library
{
    public static class Constants
    {
        public static class ServiceConfigurationKeys
        {
            public static readonly ServiceConfigurationKey Chain = ServiceConfigurationKey.From<Chain>();
            public static readonly ServiceConfigurationKey Ship = ServiceConfigurationKey.From<Ship>();

            public static class ShipParts
            {
                public static readonly ServiceConfigurationKey Hull = ServiceConfigurationKey.From<Hull>();
            }

            public static class Players
            {
                public static readonly ServiceConfigurationKey UserPlayer = ServiceConfigurationKey.From<UserPlayer>();
            }
        }

        public static class LayersContexts
        {
            public static readonly LayerContext Chunks = new LayerContext()
            {
                Group = new SingleLayerGroup(0)
            };

            public static readonly LayerContext Players = new LayerContext()
            {
                Group = new SingleLayerGroup(1)
            };

            public static readonly LayerContext Ships = new LayerContext()
            {
                Group = new SingleLayerGroup(2)
            };

            public static readonly LayerContext Chains = new LayerContext()
            {
                Group = new SingleLayerGroup(3)
            };
        }

        public static class Channels
        {
            public static readonly Int16 MainChannel = 0;
        }

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
            }

            public static class UserPlayer
            {
                public static readonly UInt32 RequestDirectionChanged = "vhr:user-player:direction:changed".xxHash();
            }
        }

        public static class MessageContexts
        {
            public static class WorldObject
            {
                public readonly static NetOutgoingMessageContext WorldInfoPingMessageContext = new NetOutgoingMessageContext()
                {
                    Method = NetDeliveryMethod.UnreliableSequenced,
                    SequenceChannel = 1
                };
            }
        }

        public static class Intervals
        {
            public static readonly Double WorldInfoPingBroadcastInterval = 50;

            /// <summary>
            /// The minimum amount of time before <see cref="AetherBodyWorldObjectMasterValidateWorldInfoChangeDetectedComponent"/>
            /// will bother checking for a change.
            /// </summary>
            public static readonly Double AetherBodyWorldObjectCleanIntervalMinimum = 150;

            /// <summary>
            /// The maximum amount of time before <see cref="AetherBodyWorldObjectMasterValidateWorldInfoChangeDetectedComponent"/>
            /// will broadcast the defined entity reguardless of dirty state.
            /// </summary>
            public static readonly Double AetherBodyWorldObjectCleanIntervalMaximum = 250;
        }

        public static class PipeIds
        {
            public static readonly Guid PlayersPipeId = new Guid(new Byte[16] { (Byte)PipeType.Players, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        }

        public static class LerpStrengths
        {
            public static readonly Single SlaveBodyLerpStrength = 1f;
        }

        public static class Thresholds
        {
            public static readonly Single SlaveBodyPositionSnapThreshold = 5f;
            public static readonly Single SlaveBodyRotationSnapThreshold = MathHelper.PiOver2;
            public static readonly Single SlaveBodyPositionDifferenceTheshold = 0.001f;
            public static readonly Single SlaveBodyRotationDifferenceTheshold = 0.0001f;

            public static readonly Single MasterBodyAngularVelocityDifferenceTheshold = 0.001f;
            public static readonly Single MasterBodyLinearVelocityDifferenceTheshold = 0.0001f;
        }
    }
}
