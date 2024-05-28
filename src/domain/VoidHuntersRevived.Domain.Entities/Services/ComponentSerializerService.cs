using Guppy.Core.Common;
using Svelto.DataStructures;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class ComponentSerializerService : IComponentSerializerService
    {
        private Dictionary<Type, ComponentSerializer> _serializers;

        public ComponentSerializerService(IFiltered<ComponentSerializer> serializers)
        {
            _serializers = serializers.ToDictionary(x => x.Type, x => x);
        }

        public FasterList<ComponentSerializer> GetInstanceComponentSerializers(IEntityType type)
        {
            List<ComponentSerializer> items = new List<ComponentSerializer>();

            foreach (Type component in type.Descriptor.Instance.componentsToBuild.Select(x => x.GetEntityComponentType()))
            {
                if (_serializers.TryGetValue(component, out var serializer))
                {
                    items.Add(serializer);
                }
            }

            FasterList<ComponentSerializer> result = new FasterList<ComponentSerializer>(items);

            return result;
        }
    }
}
