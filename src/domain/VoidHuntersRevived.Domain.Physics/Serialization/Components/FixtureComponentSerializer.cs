using Guppy.Core.Logging.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using FixtureComponent = VoidHuntersRevived.Domain.Physics.Common.Components.Fixture;

namespace VoidHuntersRevived.Domain.Physics.Serialization.Components
{
    public sealed class FixtureComponentSerializer(IEntityQueryService entityQueryService, ILogger logger) : ComponentSerializer<FixtureComponent>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ILogger _logger = logger;
        protected override FixtureComponent Read(in DeserializationOptions options, ref EntityReader reader, in InitializingEntity entity)
        {
            EntityLocalId bodyLocalId = this._entityQueryService.GetLocalId(options.Owner);

            this._logger.Verbose("Deserializing Fixture - Id = {Id}, BodyLocalId = {BodyLocalId}", entity.GlobalId, bodyLocalId);

            Fix64 localRotation = reader.Read<Fix64>();
            FixTransform2D localTransform = reader.Read<FixTransform2D>();

            FixtureComponent instance = new(bodyLocalId);
            instance.SetLocalRotationTransform(localRotation, localTransform);

            return instance;
        }

        protected override void Write(ref EntityWriter writer, in Entity entity, in FixtureComponent instance, in SerializationOptions options)
        {
            writer.Write(instance.LocalRotation);
            writer.Write(instance.LocalTransform);
        }
    }
}