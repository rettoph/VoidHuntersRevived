using Moq;

namespace VoidHuntersRevived.Tests.Common
{
    public class ServiceProviderMocker
    {
        private readonly IServiceResolver[] _resolvers;

        internal ServiceProviderMocker(IServiceResolver[] resolvers)
        {
            _resolvers = resolvers;
        }

        public T Get<T>()
            where T : class
        {
            IServiceResolver<T>? resolver = _resolvers.OfType<IServiceResolver<T>>().FirstOrDefault();
            if (resolver is not null)
            {
                return resolver.GetInstance(this);
            }

            Mock<T> mock = new();
            return mock.Object;
        }

        public Lazy<T> GetLazy<T>()
            where T : class
        {
            return new Lazy<T>(() => this.Get<T>());
        }

        public IEnumerable<T> GetAll<T>()
            where T : class
        {
            return _resolvers.OfType<IServiceResolver<T>>().Select(x => x.GetInstance(this));
        }
    }
}
