using System.Runtime.InteropServices;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class EntityTypeKeyService : IEntityTypeKeyService
    {
        private readonly IEntityTypeProviderService _entityTypeProviderService;
        private readonly Dictionary<Type, object> _keysByType;
        private readonly Dictionary<IKey<IEntityType>, IKey<IEntityType>[]> _keysByImplementation;

        public EntityTypeKeyService(IEntityTypeProviderService entityTypeProviderService)
        {
            _entityTypeProviderService = entityTypeProviderService;

            _keysByType = new Dictionary<Type, object>();
            _keysByImplementation = new Dictionary<IKey<IEntityType>, IKey<IEntityType>[]>();
        }

        public IKey<T>[] GetAll<T>()
            where T : IEntityType
        {
            ref object? keys = ref CollectionsMarshal.GetValueRefOrAddDefault(_keysByType, typeof(T), out bool exists);

            if (exists)
            {
                return (IKey<T>[])keys!;
            }

            IKey<T>[] newKeys = _entityTypeProviderService.GetAllByType<T>().Select(x => (IKey<T>)x.Type.Key).ToArray();
            keys = newKeys;

            return newKeys;
        }

        public IKey<IEntityType>[] GetAllImplementing(IKey<IEntityType> key)
        {
            ref IKey<IEntityType>[]? keys = ref CollectionsMarshal.GetValueRefOrAddDefault(_keysByImplementation, key, out bool exists);

            if (exists)
            {
                return keys!;
            }

            keys = _entityTypeProviderService.GetAllByKey(key).Select(x => x.Type.Key).ToArray();
            return keys;
        }
    }
}
