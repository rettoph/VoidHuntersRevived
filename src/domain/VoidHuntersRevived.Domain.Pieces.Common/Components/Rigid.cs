using Guppy.Core.Resources.Common;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Physics.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components
{
    public readonly struct Rigid(Resource<IBodyTemplate> template) : IEntityComponent, IPieceComponent
    {
        public readonly Resource<IBodyTemplate> Template = template;
    }
}
