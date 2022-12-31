﻿using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Identity;
using Guppy.Network.Messages;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Serialization.NetSerializers
{
    [AutoLoad(0)]
    internal sealed class UserPilotNetSerializer : NetSerializer<UserPilot>
    {
        private INetSerializer<UserAction> _userActionSerializer = default!;

        public override void Initialize(INetSerializerProvider serializers)
        {
            base.Initialize(serializers);

            _userActionSerializer = serializers.Get<UserAction>();
        }

        public override UserPilot Deserialize(NetDataReader reader)
        {
            var pilotId = reader.GetUShort();
            var user = _userActionSerializer.Deserialize(reader);

            return new UserPilot(pilotId, user);
        }

        public override void Serialize(NetDataWriter writer, in UserPilot instance)
        {
            writer.Put(instance.PilotId);
            _userActionSerializer.Serialize(writer, instance.User);
        }
    }
}
