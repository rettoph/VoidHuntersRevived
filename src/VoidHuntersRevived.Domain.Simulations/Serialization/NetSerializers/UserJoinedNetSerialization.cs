using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Identity.Dtos;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using VoidHuntersRevived.Common.Simulations.Events;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class UserJoinedNetSerialization : NetSerializer<UserJoined>
    {
        private INetSerializer<UserDto> _userDtoSerializer = null!;

        public override void Initialize(INetSerializerProvider serializers)
        {
            base.Initialize(serializers);

            _userDtoSerializer = serializers.Get<UserDto>();
        }


        public override UserJoined Deserialize(NetDataReader reader)
        {
            UserJoined instance = new UserJoined()
            {
                UserDto = _userDtoSerializer.Deserialize(reader)
            };

            return instance;
        }

        public override void Serialize(NetDataWriter writer, in UserJoined instance)
        {
            _userDtoSerializer.Serialize(writer, instance.UserDto);
        }
    }
}
