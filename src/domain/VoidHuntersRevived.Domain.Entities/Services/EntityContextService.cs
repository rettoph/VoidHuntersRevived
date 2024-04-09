using Guppy;
using Guppy.Resources.Providers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal class EntityContextService : GlobalComponent, IEntityContextService
    {
        private readonly IResourceProvider _resources;

        private EntityContext[] _contexts;
        private Dictionary<Type, EntityContext[]> _byDescriptor;
        private Dictionary<string, EntityContext> _byKey;

        public EntityContextService(IResourceProvider resources, IEnumerable<EntityContext> contexts)
        {
            _resources = resources;
            _contexts = contexts.ToArray();
            _byDescriptor = null!;
            _byKey = null!;
        }

        protected override void Initialize(IGlobalComponent[] components)
        {
            base.Initialize(components);

            _resources.Initialize(components);
            _contexts = _contexts.Concat(_resources.GetAll<EntityContext>().Select(x => x.Item2)).ToArray();
            _byDescriptor = new Dictionary<Type, EntityContext[]>();
            _byKey = _contexts.ToDictionary(x => x.Key, x => x);
        }

        public EntityContext[] All<TDescriptor>() where TDescriptor : VoidHuntersEntityDescriptor
        {
            ref EntityContext[]? pieces = ref CollectionsMarshal.GetValueRefOrAddDefault(_byDescriptor, typeof(TDescriptor), out bool exists);

            if (exists == false)
            {
                pieces = _contexts.Where(x => x.Descriptor.GetType().IsAssignableFrom(typeof(TDescriptor))).ToArray();
            }

            return pieces!;
        }

        public EntityContext[] All()
        {
            return _contexts;
        }

        public EntityContext GetByKey(string key)
        {
            return _byKey[key];
        }

        public bool TryGetByKey(string key, [MaybeNullWhen(false)] out EntityContext piece)
        {
            return _byKey.TryGetValue(key, out piece);
        }
    }
}
