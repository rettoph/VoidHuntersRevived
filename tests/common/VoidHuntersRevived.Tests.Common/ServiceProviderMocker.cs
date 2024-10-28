using Guppy.Tests.Common;

namespace VoidHuntersRevived.Tests.Common
{
    public class ServiceProviderMocker
    {
        private readonly List<object> _services = [];

        public void Add(object service)
        {
            _services.Add(service);
        }

        public void AddRange(IEnumerable<object> services)
        {
            _services.AddRange(services);
        }

        public T GetInstance<T>()
            where T : class
        {
            return _services.OfType<T>().First();
        }

        public T Get<T>()
            where T : class
        {
            T? result = _services.OfType<T>().FirstOrDefault();
            if (result is not null)
            {
                return result;
            }

            IServiceBuilder<T>? builder = _services.OfType<IServiceBuilder<T>>().FirstOrDefault();
            if (builder is not null)
            {
                result = builder.Build(this);
                _services.Add(result);
                return result;
            }

            Mocker<T>? mocker = _services.OfType<Mocker<T>>().FirstOrDefault();
            if (mocker is not null)
            {
                return mocker.GetInstance();
            }

            return this.GetMocker<T>();
        }

        public Lazy<T> GetLazy<T>()
            where T : class
        {
            return new Lazy<T>(() => this.Get<T>());
        }

        public IEnumerable<T> GetAll<T>()
        {
            return _services.OfType<T>();
        }

        public Mocker<T> GetMocker<T>()
            where T : class
        {
            Mocker<T>? result = _services.OfType<Mocker<T>>().FirstOrDefault();
            if (result is null)
            {
                result = new Mocker<T>();
                _services.Add(result);
            }

            return result;
        }
    }
}
