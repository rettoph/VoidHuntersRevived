using Autofac;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Providers;

namespace VoidHuntersRevived.Domain.Entities.Common.Initializers
{
    public abstract class BaseEntityTypeProviderInitializer : IEntityTypeProviderInitializer
    {
        private readonly List<KeyValuePair<Func<IEntityTypeProvider, bool>, Action<IEntityTypeProvider>>> _typeInitializers;
        private readonly List<KeyValuePair<Func<IEntityTypeProvider, bool>, EntityInitializerDelegate>> _initializers;
        private readonly List<KeyValuePair<Func<IEntityTypeProvider, bool>, DisposeEntityInitializerDelegate>> _disposers;

        public int Order { get; set; }

        public BaseEntityTypeProviderInitializer()
        {
            _typeInitializers = new List<KeyValuePair<Func<IEntityTypeProvider, bool>, Action<IEntityTypeProvider>>>();
            _initializers = new List<KeyValuePair<Func<IEntityTypeProvider, bool>, EntityInitializerDelegate>>();
            _disposers = new List<KeyValuePair<Func<IEntityTypeProvider, bool>, DisposeEntityInitializerDelegate>>();

            this.Order = 0;
        }

        protected BaseEntityTypeProviderInitializer WithEntityTypeInitializer(Func<IEntityTypeProvider, bool> entityTypeFilter, Action<IEntityTypeProvider>? initializer)
        {
            if (initializer is null)
            {
                return this;
            }

            _typeInitializers.Add(new KeyValuePair<Func<IEntityTypeProvider, bool>, Action<IEntityTypeProvider>>(entityTypeFilter, initializer));

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

        protected BaseEntityTypeProviderInitializer WithEntityInitializer(Func<IEntityTypeProvider, bool> entityTypeFilter, EntityInitializerDelegate? initializer)
        {
            if (initializer is null)
            {
                return this;
            }

            _initializers.Add(new KeyValuePair<Func<IEntityTypeProvider, bool>, EntityInitializerDelegate>(entityTypeFilter, initializer));

            return this;
        }

        protected BaseEntityTypeProviderInitializer WithEntityInitializer(IKey<IEntityType> entityTypeKey, EntityInitializerDelegate? initializer)
        {
            return this.WithEntityInitializer(entityTypeProvider => entityTypeProvider.Implements(entityTypeKey), initializer);
        }

        protected BaseEntityTypeProviderInitializer WithEntityInitializer(string entityTypeKey, EntityInitializerDelegate? initializer)
        {
            return this.WithEntityInitializer(Key.GetByName<IEntityType>(entityTypeKey), initializer);
        }
        protected BaseEntityTypeProviderInitializer WithInstanceEntityInitializer<T>(EntityInitializerDelegate? initializer)
            where T : IEntityType
        {
            return this.WithEntityInitializer(entityTypeProvider => entityTypeProvider.Type.Key is IKey<T>, initializer);
        }

        protected BaseEntityTypeProviderInitializer WithEntityDisposer(Func<IEntityTypeProvider, bool> entityTypeFilter, DisposeEntityInitializerDelegate? disposer)
        {
            if (disposer is null)
            {
                return this;
            }

            _disposers.Add(new KeyValuePair<Func<IEntityTypeProvider, bool>, DisposeEntityInitializerDelegate>(entityTypeFilter, disposer));

            return this;
        }

        protected BaseEntityTypeProviderInitializer WithEntityDisposer(IKey<IEntityType> entityTypeKey, DisposeEntityInitializerDelegate? disposer)
        {
            return this.WithEntityDisposer(entityTypeProvider => entityTypeProvider.Implements(entityTypeKey), disposer);
        }

        protected BaseEntityTypeProviderInitializer WithEntityDisposer(string entityTypeKey, DisposeEntityInitializerDelegate? disposer)
        {
            return this.WithEntityDisposer(Key.GetByName<IEntityType>(entityTypeKey), disposer);
        }
        protected BaseEntityTypeProviderInitializer WithEntityDisposer<T>(DisposeEntityInitializerDelegate? disposer)
            where T : IEntityType
        {
            return this.WithEntityDisposer(entityTypeProvider => entityTypeProvider.Type.Key is IKey<T>, disposer);
        }


        void IEntityTypeProviderInitializer.InitializeEntityTypeProvider(IEntityTypeProvider entityTypeProvider)
        {
            foreach (Action<IEntityTypeProvider> initializer in _typeInitializers.Where(x => x.Key(entityTypeProvider)).Select(x => x.Value))
            {
                initializer(entityTypeProvider);
            }
        }

        DisposeEntityInitializerDelegate? IEntityTypeProviderInitializer.GetEntityDisposer(IEntityTypeProvider entityTypeProvider)
        {
            DisposeEntityInitializerDelegate? result = default;

            foreach ((Func<IEntityTypeProvider, bool> filter, DisposeEntityInitializerDelegate disposer) in _disposers)
            {
                if (filter(entityTypeProvider))
                {
                    result += disposer;
                }
            }

            return result;
        }

        EntityInitializerDelegate? IEntityTypeProviderInitializer.GetEntityInitializer(IEntityTypeProvider entityTypeProvider)
        {
            EntityInitializerDelegate? result = default;

            foreach ((Func<IEntityTypeProvider, bool> filter, EntityInitializerDelegate initializer) in _initializers)
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
