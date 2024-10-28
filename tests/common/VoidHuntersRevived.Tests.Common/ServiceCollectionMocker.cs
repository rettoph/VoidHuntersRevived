using Guppy.Tests.Common;

namespace VoidHuntersRevived.Tests.Common
{
    public class ServiceCollectionMocker
    {
        private readonly List<Mocker> _mockers = [];
        private readonly List<IServiceBuilder> _builders = [];
        private readonly List<IServiceResolver> _resolvers = [];

        public ServiceCollectionMocker RegisterFactory<T>(Func<ServiceProviderMocker, T> factory)
            where T : class
        {
            _resolvers.Add(new ServiceResolver<T>(factory));

            return this;
        }

        public ServiceCollectionMocker RegisterBuilder<T>(IServiceBuilder<T> builder)
            where T : class
        {
            _builders.Add(builder);
            _resolvers.Add(new ServiceResolver<T>(builder.Build));

            return this;
        }

        public Mocker<T> RegisterMocker<T>(Mocker<T> mocker)
            where T : class
        {
            _mockers.Add(mocker);
            _resolvers.Add(new ServiceResolver<T>(services => mocker.GetInstance()));

            return mocker;
        }

        public Mocker<T> RegisterMocker<T>()
            where T : class
        {
            return this.RegisterMocker(new Mocker<T>());
        }

        public T GetBuilder<T>()
            where T : IServiceBuilder
        {
            return _builders.OfType<T>().First();
        }

        public T GetMocker<T>()
            where T : Mocker
        {
            return _mockers.OfType<T>().First();
        }

        public ServiceProviderMocker Build()
        {
            return new ServiceProviderMocker(_resolvers.ToArray());
        }

        private class ServiceResolver<T>(Func<ServiceProviderMocker, T> resolver) : IServiceResolver<T>
            where T : class
        {
            private bool _resolving = false;
            private readonly Func<ServiceProviderMocker, T> _resolver = resolver;
            private T? _instance;

            public Type Type => typeof(T);

            public T GetInstance(ServiceProviderMocker services)
            {
                if (_instance is not null)
                {
                    return _instance;
                }

                if (_resolving == true)
                { // Circular dependency?
                    throw new NotImplementedException();
                }

                _resolving = true;
                _instance = _resolver(services);
                _resolving = false;

                return _instance;
            }
        }
    }
}
