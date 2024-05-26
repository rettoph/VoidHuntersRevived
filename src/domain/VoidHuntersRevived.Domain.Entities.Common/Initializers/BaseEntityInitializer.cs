using Autofac;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Common.Initializers
{
    public abstract class BaseEntityInitializer : IEntityInitializer
    {
        private readonly HashSet<Func<IEntityType, bool>> _filters;
        private readonly HashSet<IEntityType> _explicitTypes;
        private readonly List<KeyValuePair<Func<IEntityType, bool>, EntityInitializerDelegate>> _instanceInitializers;
        private readonly List<KeyValuePair<Func<IEntityType, bool>, DisposeEntityInitializerDelegate>> _instanceDisposers;
        private readonly List<KeyValuePair<Func<IEntityType, bool>, EntityInitializerDelegate>> _typeInitializers;
        private readonly List<KeyValuePair<Func<IEntityType, bool>, DisposeEntityInitializerDelegate>> _typeDisposers;

        public int Order { get; set; }

        IEntityType[] IEntityInitializer.ExplicitEntityTypes => _explicitTypes.ToArray();

        public BaseEntityInitializer(params IEntityType[] explicitTypes)
        {
            _filters = new HashSet<Func<IEntityType, bool>>();
            _explicitTypes = new HashSet<IEntityType>(explicitTypes);

            _instanceInitializers = new List<KeyValuePair<Func<IEntityType, bool>, EntityInitializerDelegate>>();
            _instanceDisposers = new List<KeyValuePair<Func<IEntityType, bool>, DisposeEntityInitializerDelegate>>();
            _typeInitializers = new List<KeyValuePair<Func<IEntityType, bool>, EntityInitializerDelegate>>();
            _typeDisposers = new List<KeyValuePair<Func<IEntityType, bool>, DisposeEntityInitializerDelegate>>();

            this.Order = 0;
        }

        protected BaseEntityInitializer WithExplicitType(IEntityType entityType)
        {
            _explicitTypes.Add(entityType);

            return this;
        }

        protected BaseEntityInitializer WithInstanceInitializer(Func<IEntityType, bool> entityTypeFilter, EntityInitializerDelegate? initializer)
        {
            if (initializer is null)
            {
                return this;
            }

            _filters.Add(entityTypeFilter);
            _instanceInitializers.Add(new KeyValuePair<Func<IEntityType, bool>, EntityInitializerDelegate>(entityTypeFilter, initializer));

            return this;
        }

        protected BaseEntityInitializer WithInstanceInitializer(IEntityType entityType, EntityInitializerDelegate? initializer)
        {
            return this.WithExplicitType(entityType).WithInstanceInitializer(x => x == entityType, initializer);
        }

        protected BaseEntityInitializer WithInstanceInitializer<TDescriptor>(EntityInitializerDelegate? initializer)
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

        protected BaseEntityInitializer WithTypeInitializer(Func<IEntityType, bool> entityTypeFilter, EntityInitializerDelegate? initializer)
        {
            if (initializer is null)
            {
                return this;
            }

            _filters.Add(entityTypeFilter);
            _typeInitializers.Add(new KeyValuePair<Func<IEntityType, bool>, EntityInitializerDelegate>(entityTypeFilter, initializer));

            return this;
        }

        protected BaseEntityInitializer WithTypeInitializer(IEntityType entityType, EntityInitializerDelegate? initializer)
        {
            return this.WithExplicitType(entityType).WithTypeInitializer(x => x == entityType, initializer);
        }

        protected BaseEntityInitializer WithTypeInitializer<TDescriptor>(EntityInitializerDelegate? initializer)
            where TDescriptor : VoidHuntersEntityDescriptor
        {
            return this.WithTypeInitializer(x => x.Descriptor.GetType().IsAssignableTo<TDescriptor>(), initializer);
        }

        protected BaseEntityInitializer WithTypeDisposer(Func<IEntityType, bool> entityTypeFilter, DisposeEntityInitializerDelegate? disposer)
        {
            if (disposer is null)
            {
                return this;
            }

            _filters.Add(entityTypeFilter);
            _typeDisposers.Add(new KeyValuePair<Func<IEntityType, bool>, DisposeEntityInitializerDelegate>(entityTypeFilter, disposer));

            return this;
        }

        protected BaseEntityInitializer WithTypeDisposer(IEntityType entityType, DisposeEntityInitializerDelegate? disposer)
        {
            return this.WithExplicitType(entityType).WithTypeDisposer(x => x == entityType, disposer);
        }

        protected BaseEntityInitializer WithTypeDisposer<TDescriptor>(DisposeEntityInitializerDelegate? disposer)
            where TDescriptor : VoidHuntersEntityDescriptor
        {
            return this.WithTypeDisposer(x => x.Descriptor.GetType().IsAssignableTo<TDescriptor>(), disposer);
        }

        bool IEntityInitializer.ShouldInitialize(IEntityType entityType)
        {
            if (_explicitTypes.Contains(entityType))
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

            foreach ((Func<IEntityType, bool> filter, DisposeEntityInitializerDelegate disposer) in _instanceDisposers)
            {
                if (filter(entityType))
                {
                    result += disposer;
                }
            }

            return result;
        }

        EntityInitializerDelegate? IEntityInitializer.InstanceInitializer(IEntityType entityType)
        {
            EntityInitializerDelegate? result = default;

            foreach ((Func<IEntityType, bool> filter, EntityInitializerDelegate initializer) in _instanceInitializers)
            {
                if (filter(entityType))
                {
                    result += initializer;
                }
            }

            return result;
        }

        DisposeEntityInitializerDelegate? IEntityInitializer.TypeDisposer(IEntityType entityType)
        {
            DisposeEntityInitializerDelegate? result = default;

            foreach ((Func<IEntityType, bool> filter, DisposeEntityInitializerDelegate disposer) in _typeDisposers)
            {
                if (filter(entityType))
                {
                    result += disposer;
                }
            }

            return result;
        }

        EntityInitializerDelegate? IEntityInitializer.TypeInitializer(IEntityType entityType)
        {
            EntityInitializerDelegate? result = default;

            foreach ((Func<IEntityType, bool> filter, EntityInitializerDelegate initializer) in _typeInitializers)
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
