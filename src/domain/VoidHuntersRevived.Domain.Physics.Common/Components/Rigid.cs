using Guppy.Core.Resources.Common;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Physics.Common.Components
{
    public readonly struct Rigid(Resource<IBodyTemplate> template) : IEntityComponent
    {
        public readonly Resource<IBodyTemplate> Template = template;
    }
}
