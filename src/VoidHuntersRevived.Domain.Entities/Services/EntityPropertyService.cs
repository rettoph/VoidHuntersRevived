using Guppy.Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal class EntityPropertyService : IEntityPropertyService
    {
        private readonly Dictionary<Type, EntityPropertyConfiguration> _configurations;
        private readonly Dictionary<int, PropertyCache> _cache;
        private int _cacheId;

        public EntityPropertyService(ISorted<IEntityPropertyLoader> loaders)
        {
            _configurations = new Dictionary<Type, EntityPropertyConfiguration>();
            _cache = new Dictionary<int, PropertyCache>();

            foreach(IEntityPropertyLoader loader in loaders)
            {
                loader.Configure(this);
            }
        }

        public void Configure<T>(Action<IEntityPropertyConfiguration<T>> configuration)
            where T : class, IEntityProperty
        {
            configuration(this.GetOrCreateConfiguration<T>());
        }

        public EntityPropertyConfiguration<T> GetOrCreateConfiguration<T>()
            where T : class, IEntityProperty
        {
            if (!_configurations.TryGetValue(typeof(T), out EntityPropertyConfiguration? configuration))
            {
                _configurations[typeof(T)] = configuration = new EntityPropertyConfiguration<T>();
            }

            return (EntityPropertyConfiguration<T>)configuration;
        }

        private static readonly MethodInfo _genericGetOrCreateMethod = typeof(EntityPropertyService)
            .GetMethod(nameof(GetOrCreateConfiguration), 1, Array.Empty<Type>()) ?? throw new Exception();

        public EntityPropertyConfiguration GetOrCreateConfiguration(Type type)
        {
            var method = _genericGetOrCreateMethod.MakeGenericMethod(type);
            return (EntityPropertyConfiguration)method.Invoke(this, Array.Empty<object>())!;
        }

        internal PropertyCache<T> Cache<T>(T instance)
            where T : class, IEntityProperty
        {
            PropertyCache<T> cached = new PropertyCache<T>(
                instance: instance,
                configuration: this.GetOrCreateConfiguration<T>(),
                id: _cacheId++);
            _cache.Add(cached.Id, cached);

            return cached;
        }

        internal T GetProperty<T>(in Property<T> id)
            where T : class, IEntityProperty
        {
            return Unsafe.As<PropertyCache<T>>(_cache[id.Id]).Instance;
        }

        internal IEnumerable<(int, T)> GetProperties<T>()
            where T : class, IEntityProperty
        {
            return _cache.Values.OfType<PropertyCache<T>>().Select(x => (x.Id, x.Instance));
        }
    }
}
