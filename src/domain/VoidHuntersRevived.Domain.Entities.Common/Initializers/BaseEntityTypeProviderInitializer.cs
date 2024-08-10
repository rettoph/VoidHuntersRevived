using Autofac;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Providers;

namespace VoidHuntersRevived.Domain.Entities.Common.Initializers
{
    public abstract class BaseEntityTypeProviderInitializer : IEntityTypeProviderInitializer
    {
        private readonly List<KeyValuePair<Func<IEntityTypeProvider, bool>, Action<IEntityTypeProvider>>> _entityTypeIntializers;
        private readonly List<KeyValuePair<Func<IEntityTypeProvider, bool>, EntityInitializerDelegate>> _instanceEntityInitializers;
        private readonly List<KeyValuePair<Func<IEntityTypeProvider, bool>, DisposeEntityInitializerDelegate>> _instanceEntityDisposers;
        private readonly List<KeyValuePair<Func<IEntityTypeProvider, bool>, EntityInitializerDelegate>> _typeEntityInitializers;
        private readonly List<KeyValuePair<Func<IEntityTypeProvider, bool>, DisposeEntityInitializerDelegate>> _typeEntityDisposers;

        public int Order { get; set; }

        public BaseEntityTypeProviderInitializer()
        {
            _entityTypeIntializers = new List<KeyValuePair<Func<IEntityTypeProvider, bool>, Action<IEntityTypeProvider>>>();
            _instanceEntityInitializers = new List<KeyValuePair<Func<IEntityTypeProvider, bool>, EntityInitializerDelegate>>();
            _instanceEntityDisposers = new List<KeyValuePair<Func<IEntityTypeProvider, bool>, DisposeEntityInitializerDelegate>>();
            _typeEntityInitializers = new List<KeyValuePair<Func<IEntityTypeProvider, bool>, EntityInitializerDelegate>>();
            _typeEntityDisposers = new List<KeyValuePair<Func<IEntityTypeProvider, bool>, DisposeEntityInitializerDelegate>>();

            this.Order = 0;
        }

        protected BaseEntityTypeProviderInitializer WithEntityTypeInitializer(Func<IEntityTypeProvider, bool> entityTypeFilter, Action<IEntityTypeProvider>? initializer)
        {
            if (initializer is null)
            {
                return this;
            }

            _entityTypeIntializers.Add(new KeyValuePair<Func<IEntityTypeProvider, bool>, Action<IEntityTypeProvider>>(entityTypeFilter, initializer));

            return this;
        }

        protected BaseEntityTypeProviderInitializer WithEntityTypeInitializer(IKey<IEntityType> entityTypeKey, Action<IEntityTypeProvider>? initializer)
        {
            return this.WithEntityTypeInitializer(entityTypeProvider => entityTypeProvider.Implements(entityTypeKey), initializer);
        }

        protected BaseEntityTypeProviderInitializer WithEntityTypeInitializer(string entityTypeKeyName, Action<IEntityTypeProvider>? initializer)
        {
            return this.WithEntityTypeInitializer(Key.GetByName<IEntityType>(entityTypeKeyName), initializer);
        }
        protected BaseEntityTypeProviderInitializer WithEntityTypeInitializer<T>(Action<IEntityTypeProvider>? initializer)
            where T : IEntityType
        {
            return this.WithEntityTypeInitializer(entityTypeProvider => entityTypeProvider.Type.Key is IKey<T>, initializer);
        }

        protected BaseEntityTypeProviderInitializer WithInstanceEntityInitializer(Func<IEntityTypeProvider, bool> entityTypeFilter, EntityInitializerDelegate? initializer)
        {
            if (initializer is null)
            {
                return this;
            }

            _instanceEntityInitializers.Add(new KeyValuePair<Func<IEntityTypeProvider, bool>, EntityInitializerDelegate>(entityTypeFilter, initializer));

            return this;
        }

        protected BaseEntityTypeProviderInitializer WithInstanceEntityInitializer(IKey<IEntityType> entityTypeKey, EntityInitializerDelegate? initializer)
        {
            return this.WithInstanceEntityInitializer(entityTypeProvider => entityTypeProvider.Implements(entityTypeKey), initializer);
        }

        protected BaseEntityTypeProviderInitializer WithInstanceEntityInitializer(string entityTypeKey, EntityInitializerDelegate? initializer)
        {
            return this.WithInstanceEntityInitializer(Key.GetByName<IEntityType>(entityTypeKey), initializer);
        }
        protected BaseEntityTypeProviderInitializer WithInstanceEntityInitializer<T>(EntityInitializerDelegate? initializer)
            where T : IEntityType
        {
            return this.WithInstanceEntityInitializer(entityTypeProvider => entityTypeProvider.Type.Key is IKey<T>, initializer);
        }

        protected BaseEntityTypeProviderInitializer WithInstanceEntityDisposer(Func<IEntityTypeProvider, bool> entityTypeFilter, DisposeEntityInitializerDelegate? disposer)
        {
            if (disposer is null)
            {
                return this;
            }

            _instanceEntityDisposers.Add(new KeyValuePair<Func<IEntityTypeProvider, bool>, DisposeEntityInitializerDelegate>(entityTypeFilter, disposer));

            return this;
        }

        protected BaseEntityTypeProviderInitializer WithInstanceEntityDisposer(IKey<IEntityType> entityTypeKey, DisposeEntityInitializerDelegate? disposer)
        {
            return this.WithInstanceEntityDisposer(entityTypeProvider => entityTypeProvider.Implements(entityTypeKey), disposer);
        }

        protected BaseEntityTypeProviderInitializer WithInstanceEntityDisposer(string entityTypeKey, DisposeEntityInitializerDelegate? disposer)
        {
            return this.WithInstanceEntityDisposer(Key.GetByName<IEntityType>(entityTypeKey), disposer);
        }
        protected BaseEntityTypeProviderInitializer WithInstanceEntityDisposer<T>(DisposeEntityInitializerDelegate? disposer)
            where T : IEntityType
        {
            return this.WithInstanceEntityDisposer(entityTypeProvider => entityTypeProvider.Type.Key is IKey<T>, disposer);
        }

        protected BaseEntityTypeProviderInitializer WithTypeEntityInitializer(Func<IEntityTypeProvider, bool> entityTypeFilter, EntityInitializerDelegate? initializer)
        {
            if (initializer is null)
            {
                return this;
            }

            _typeEntityInitializers.Add(new KeyValuePair<Func<IEntityTypeProvider, bool>, EntityInitializerDelegate>(entityTypeFilter, initializer));

            return this;
        }

        protected BaseEntityTypeProviderInitializer WithTypeEntityInitializer(IKey<IEntityType> entityTypeKey, EntityInitializerDelegate? initializer)
        {
            return this.WithTypeEntityInitializer(entityTypeProvider => entityTypeProvider.Implements(entityTypeKey), initializer);
        }
        protected BaseEntityTypeProviderInitializer WithTypeEntityInitializer(string entityTypeKey, EntityInitializerDelegate? initializer)
        {
            return this.WithTypeEntityInitializer(Key.GetByName<IEntityType>(entityTypeKey), initializer);
        }
        protected BaseEntityTypeProviderInitializer WithTypeEntityInitializer<T>(EntityInitializerDelegate? initializer)
            where T : IEntityType
        {
            return this.WithTypeEntityInitializer(entityTypeProvider => entityTypeProvider.Type.Key is IKey<T>, initializer);
        }

        protected BaseEntityTypeProviderInitializer WithTypeEntityDisposer(Func<IEntityTypeProvider, bool> entityTypeFilter, DisposeEntityInitializerDelegate? disposer)
        {
            if (disposer is null)
            {
                return this;
            }

            _typeEntityDisposers.Add(new KeyValuePair<Func<IEntityTypeProvider, bool>, DisposeEntityInitializerDelegate>(entityTypeFilter, disposer));

            return this;
        }

        protected BaseEntityTypeProviderInitializer WithTypeEntityDisposer(IKey<IEntityType> entityTypeKey, DisposeEntityInitializerDelegate? disposer)
        {
            return this.WithTypeEntityDisposer(entityTypeProvider => entityTypeProvider.Implements(entityTypeKey), disposer);
        }

        protected BaseEntityTypeProviderInitializer WithTypeEntityDisposer(string entityTypeKey, DisposeEntityInitializerDelegate? disposer)
        {
            return this.WithTypeEntityDisposer(Key.GetByName<IEntityType>(entityTypeKey), disposer);
        }
        protected BaseEntityTypeProviderInitializer WithTypeEntityDisposer<T>(DisposeEntityInitializerDelegate? disposer)
            where T : IEntityType
        {
            return this.WithTypeEntityDisposer(entityTypeProvider => entityTypeProvider.Type.Key is IKey<T>, disposer);
        }


        void IEntityTypeProviderInitializer.InitializeEntityTypeProvider(IEntityTypeProvider entityTypeProvider)
        {
            foreach (Action<IEntityTypeProvider> initializer in _entityTypeIntializers.Where(x => x.Key(entityTypeProvider)).Select(x => x.Value))
            {
                initializer(entityTypeProvider);
            }
        }

        DisposeEntityInitializerDelegate? IEntityTypeProviderInitializer.GetInstanceEntityDisposer(IEntityTypeProvider entityTypeProvider)
        {
            DisposeEntityInitializerDelegate? result = default;

            foreach ((Func<IEntityTypeProvider, bool> filter, DisposeEntityInitializerDelegate disposer) in _instanceEntityDisposers)
            {
                if (filter(entityTypeProvider))
                {
                    result += disposer;
                }
            }

            return result;
        }

        EntityInitializerDelegate? IEntityTypeProviderInitializer.GetInstanceEntityInitializer(IEntityTypeProvider entityTypeProvider)
        {
            EntityInitializerDelegate? result = default;

            foreach ((Func<IEntityTypeProvider, bool> filter, EntityInitializerDelegate initializer) in _instanceEntityInitializers)
            {
                if (filter(entityTypeProvider))
                {
                    result += initializer;
                }
            }

            return result;
        }

        DisposeEntityInitializerDelegate? IEntityTypeProviderInitializer.GetTypeEntityDisposer(IEntityTypeProvider entityTypeProvider)
        {
            DisposeEntityInitializerDelegate? result = default;

            foreach ((Func<IEntityTypeProvider, bool> filter, DisposeEntityInitializerDelegate disposer) in _typeEntityDisposers)
            {
                if (filter(entityTypeProvider))
                {
                    result += disposer;
                }
            }

            return result;
        }

        EntityInitializerDelegate? IEntityTypeProviderInitializer.GetTypeEntityInitializer(IEntityTypeProvider entityTypeProvider)
        {
            EntityInitializerDelegate? result = default;

            foreach ((Func<IEntityTypeProvider, bool> filter, EntityInitializerDelegate initializer) in _typeEntityInitializers)
            {
                if (filter(entityTypeProvider))
                {
                    result += initializer;
                }
            }

            return result;
        }
    }
}
