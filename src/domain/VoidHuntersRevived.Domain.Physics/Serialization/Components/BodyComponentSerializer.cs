using Serilog;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using BodyComponent = VoidHuntersRevived.Domain.Physics.Common.Components.Body;

namespace VoidHuntersRevived.Domain.Physics.Serialization.Components
{
    public sealed class BodyComponentSerializer(ILogger logger) : ComponentSerializer<BodyComponent>
    {
        private readonly ILogger _logger = logger;

        protected override BodyComponent Read(in DeserializationOptions options, ref EntityReader reader, in InitializingEntity entity)
        {
            _logger.Verbose("Deserializing Body - Id = {Id}, BodyLocalId = {BodyLocalId}", entity.GlobalId, entity.LocalId);

            Fix64 rotation = reader.Read<Fix64>();
            FixTransform2D transform = reader.Read<FixTransform2D>();

            return new(entity.LocalId, rotation, transform);
        }

        protected override void Write(ref EntityWriter writer, in Entity entity, in BodyComponent instance, in SerializationOptions options)
        {
            writer.Write(instance.Rotation);
            writer.Write(instance.Transform);
        }
    }
}
