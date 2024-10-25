using Guppy.Core.Resources.Common;
using Guppy.Core.Serialization.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Physics.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components
{
    [PolymorphicJsonType<IEntityComponent>(nameof(Rigid))]
    public readonly struct Rigid(Resource<IBodyTemplate> template) : IEntityComponent, IPieceComponent
    {
        public readonly Resource<IBodyTemplate> Template = template;
    }
}
