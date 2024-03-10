using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities.Initializers
{
    public abstract class BaseEntityInitializer : IEntityInitializer
    {
        private readonly HashSet<Func<IEntityType, bool>> _filters;
        private readonly HashSet<IEntityType> _explicitTypes;
        private readonly List<KeyValuePair<Func<IEntityType, bool>, InstanceEntityInitializerDelegate>> _instanceInitializers;
        private readonly List<KeyValuePair<Func<IEntityType, bool>, DisposeEntityInitializerDelegate>> _instanceDisposers;
        private readonly List<KeyValuePair<Func<IEntityType, bool>, StaticEntityInitializerDelegate>> _staticInitializers;
        private readonly List<KeyValuePair<Func<IEntityType, bool>, DisposeEntityInitializerDelegate>> _staticDisposers;

        IEntityType[] IEntityInitializer.ExplicitEntityTypes => _explicitTypes.ToArray();

        public BaseEntityInitializer(params IEntityType[] explicitTypes)
        {
            _filters = new HashSet<Func<IEntityType, bool>>();
            _explicitTypes = new HashSet<IEntityType>(explicitTypes);

            _instanceInitializers = new List<KeyValuePair<Func<IEntityType, bool>, InstanceEntityInitializerDelegate>>();
            _instanceDisposers = new List<KeyValuePair<Func<IEntityType, bool>, DisposeEntityInitializerDelegate>>();
            _staticInitializers = new List<KeyValuePair<Func<IEntityType, bool>, StaticEntityInitializerDelegate>>();
            _staticDisposers = new List<KeyValuePair<Func<IEntityType, bool>, DisposeEntityInitializerDelegate>>();
        }

        protected BaseEntityInitializer WithExplicitType(IEntityType entityType)
        {
            _explicitTypes.Add(entityType);

            return this;
        }

        protected BaseEntityInitializer WithInstanceInitializer(Func<IEntityType, bool> entityTypeFilter, InstanceEntityInitializerDelegate? initializer)
        {
            if(initializer is null)
            {
                return this;
            }

            _filters.Add(entityTypeFilter);
            _instanceInitializers.Add(new KeyValuePair<Func<IEntityType, bool>, InstanceEntityInitializerDelegate>(entityTypeFilter, initializer));

            return this;
        }

        protected BaseEntityInitializer WithInstanceInitializer(IEntityType entityType, InstanceEntityInitializerDelegate? initializer)
        {
            return this.WithExplicitType(entityType).WithInstanceInitializer(x => x == entityType, initializer);
        }

        protected BaseEntityInitializer WithInstanceInitializer<TDescriptor>(InstanceEntityInitializerDelegate? initializer)
            where TDescriptor : VoidHuntersEntityDescriptor
        {
            return this.WithInstanceInitializer(x => x.Descriptor.GetType().IsAssignableTo<TDescriptor>(), initializer);
        }

        protected BaseEntityInitializer WithInstanceDisposer(Func<IEntityType, bool> entityTypeFilter, DisposeEntityInitializerDelegate? disposer)
        {
            if (disposer is null)
            {
                return this;
            }

            _filters.Add(entityTypeFilter);
            _instanceDisposers.Add(new KeyValuePair<Func<IEntityType, bool>, DisposeEntityInitializerDelegate>(entityTypeFilter, disposer));

            return this;
        }

        protected BaseEntityInitializer WithInstanceDisposer(IEntityType entityType, DisposeEntityInitializerDelegate? disposer)
        {
            return this.WithExplicitType(entityType).WithInstanceDisposer(x => x == entityType, disposer);
        }

        protected BaseEntityInitializer WithInstanceDisposer<TDescriptor>(DisposeEntityInitializerDelegate? disposer)
            where TDescriptor : VoidHuntersEntityDescriptor
        {
            return this.WithInstanceDisposer(x => x.Descriptor.GetType().IsAssignableTo<TDescriptor>(), disposer);
        }

        protected BaseEntityInitializer WithStaticInitializer(Func<IEntityType, bool> entityTypeFilter, StaticEntityInitializerDelegate? initializer)
        {
            if (initializer is null)
            {
                return this;
            }

            _filters.Add(entityTypeFilter);
            _staticInitializers.Add(new KeyValuePair<Func<IEntityType, bool>, StaticEntityInitializerDelegate>(entityTypeFilter, initializer));

            return this;
        }

        protected BaseEntityInitializer WithStaticInitializer(IEntityType entityType, StaticEntityInitializerDelegate? initializer)
        {
            return this.WithExplicitType(entityType).WithStaticInitializer(x => x == entityType, initializer);
        }

        protected BaseEntityInitializer WithStaticInitializer<TDescriptor>(StaticEntityInitializerDelegate? initializer)
            where TDescriptor : VoidHuntersEntityDescriptor
        {
            return this.WithStaticInitializer(x => x.Descriptor.GetType().IsAssignableTo<TDescriptor>(), initializer);
        }

        protected BaseEntityInitializer WithStaticDisposer(Func<IEntityType, bool> entityTypeFilter, DisposeEntityInitializerDelegate? disposer)
        {
            if (disposer is null)
            {
                return this;
            }

            _filters.Add(entityTypeFilter);
            _staticDisposers.Add(new KeyValuePair<Func<IEntityType, bool>, DisposeEntityInitializerDelegate>(entityTypeFilter, disposer));

            return this;
        }

        protected BaseEntityInitializer WithStaticDisposer(IEntityType entityType, DisposeEntityInitializerDelegate? disposer)
        {
            return this.WithExplicitType(entityType).WithStaticDisposer(x => x == entityType, disposer);
        }

        protected BaseEntityInitializer WithStaticDisposer<TDescriptor>(DisposeEntityInitializerDelegate? disposer)
            where TDescriptor : VoidHuntersEntityDescriptor
        {
            return this.WithStaticDisposer(x => x.Descriptor.GetType().IsAssignableTo<TDescriptor>(), disposer);
        }

        bool IEntityInitializer.ShouldInitialize(IEntityType entityType)
        {
            if(_explicitTypes.Contains(entityType))
            {
                return true;
            }

            foreach (Func<IEntityType, bool> filter in _filters)
            {
                if (filter(entityType))
                {
                    return true;
                }
            }

            return false;
        }

        DisposeEntityInitializerDelegate? IEntityInitializer.InstanceDisposer(IEntityType entityType)
        {
            DisposeEntityInitializerDelegate? result = default;

            foreach((Func<IEntityType, bool> filter, DisposeEntityInitializerDelegate disposer) in _instanceDisposers)
            {
                if(filter(entityType))
                {
                    result += disposer;
                }
            }

            return result;
        }

        InstanceEntityInitializerDelegate? IEntityInitializer.InstanceInitializer(IEntityType entityType)
        {
            InstanceEntityInitializerDelegate? result = default;

            foreach ((Func<IEntityType, bool> filter, InstanceEntityInitializerDelegate initializer) in _instanceInitializers)
            {
                if (filter(entityType))
                {
                    result += initializer;
                }
            }

            return result;
        }

        DisposeEntityInitializerDelegate? IEntityInitializer.StaticDisposer(IEntityType entityType)
        {
            DisposeEntityInitializerDelegate? result = default;

            foreach ((Func<IEntityType, bool> filter, DisposeEntityInitializerDelegate disposer) in _staticDisposers)
            {
                if (filter(entityType))
                {
                    result += disposer;
                }
            }

            return result;
        }

        StaticEntityInitializerDelegate? IEntityInitializer.StaticInitializer(IEntityType entityType)
        {
            StaticEntityInitializerDelegate? result = default;

            foreach ((Func<IEntityType, bool> filter, StaticEntityInitializerDelegate initializer) in _staticInitializers)
            {
                if (filter(entityType))
                {
                    result += initializer;
                }
            }

            return result;
        }
    }
}
