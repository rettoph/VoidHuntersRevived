using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Descriptors
{
    public class VoidHuntersEntityDescriptor : IEntityDescriptor
    {
        public IComponentBuilder[] componentsToBuild => [];
    }
}