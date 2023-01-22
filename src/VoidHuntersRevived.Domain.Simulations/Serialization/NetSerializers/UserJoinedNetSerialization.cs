using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Identity.Claims;
using Guppy.Network.Messages;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Simulations.Events;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class UserJoinedNetSerialization : NetSerializer<UserJoined>
    {
        public override UserJoined Deserialize(NetDataReader reader)
        {
            var instance = new UserJoined()
            {
                Id = reader.GetInt(),
                Claims = new Claim[reader.GetInt()]
            };

            for (var i = 0; i < instance.Claims.Length; i++)
            {
                instance.Claims[i] = Claim.Deserialize(reader);
            }

            return instance;
        }

        public override void Serialize(NetDataWriter writer, in UserJoined instance)
        {
            writer.Put(instance.Id);
            writer.Put(instance.Claims.Length);

            foreach (Claim claim in instance.Claims)
            {
                claim.Serialize(writer);
            }
        }
    }
}
