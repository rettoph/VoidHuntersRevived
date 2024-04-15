using Guppy.Core.Common.Attributes;
using Guppy.Core.Network;
using Guppy.Core.Network.Identity.Dtos;
using Guppy.Core.Network.Services;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Simulations.Common.Events;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class UserJoinedNetSerialization : NetSerializer<UserJoined>
    {
        private INetSerializer<UserDto> _userDtoSerializer = null!;

        public override void Initialize(INetSerializerService serializers)
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
