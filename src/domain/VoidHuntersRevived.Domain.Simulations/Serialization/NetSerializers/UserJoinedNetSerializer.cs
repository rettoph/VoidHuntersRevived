using Guppy.Core.Network.Common.Dtos;
using Guppy.Core.Network.Common.Serialization;
using Guppy.Core.Network.Common.Services;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Simulations.Common.Events;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    internal sealed class UserJoinedNetSerializer : NetSerializer<UserJoined>
    {
        private INetSerializer<UserDto> _userDtoSerializer = null!;

        public override void Initialize(INetSerializerService serializers)
        {
            base.Initialize(serializers);

            this._userDtoSerializer = serializers.Get<UserDto>();
        }


        public override UserJoined Deserialize(NetDataReader reader)
        {
            UserJoined instance = new()
            {
                UserDto = this._userDtoSerializer.Deserialize(reader)
            };

            return instance;
        }

        public override void Serialize(NetDataWriter writer, in UserJoined instance) => this._userDtoSerializer.Serialize(writer, instance.UserDto);
    }
}