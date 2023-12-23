using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Identity.Claims;
using LiteNetLib.Utils;
using VoidHuntersRevived.Common.Simulations.Events;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class UserJoinedNetSerialization : NetSerializer<UserJoined>
    {
        public override UserJoined Deserialize(NetDataReader reader)
        {
            var instance = new UserJoined()
            {
                UserId = reader.GetInt(),
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
            writer.Put(instance.UserId);
            writer.Put(instance.Claims.Length);

            foreach (Claim claim in instance.Claims)
            {
                claim.Serialize(writer);
            }
        }
    }
}
