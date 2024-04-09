using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityTypeService : IEntityTypeService
    {
        private Dictionary<Id<IEntityType>, IEntityType> _types;
        private Dictionary<Type, object> _byDescriptor;

        public EntityTypeService(IEntityTypeInitializerService initializers)
        {
            _types = initializers.GetAll().ToDictionary(x => x.Type.Id, x => x.Type);
            _byDescriptor = new Dictionary<Type, object>();
        }

        public IEntityType GetById(Id<IEntityType> id)
        {
            return _types[id];
        }

        public IEnumerable<IEntityType> GetAll()
        {
            return _types.Values;
        }

        public IEntityType<T>[] GetAll<T>() where T : VoidHuntersEntityDescriptor
        {
            ref object? array = ref CollectionsMarshal.GetValueRefOrAddDefault(_byDescriptor, typeof(T), out bool exists);
            if (exists)
            {
                return (IEntityType<T>[])array!;
            }

            array = _types.Values.OfType<IEntityType<T>>().ToArray();
            return (IEntityType<T>[])array;
        }
    }
}
