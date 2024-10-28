namespace VoidHuntersRevived.Tests.Common
{
    public abstract class ServiceBuilder<T> : IServiceBuilder<T>
        where T : class
    {
        private T? _instance;

        T IServiceBuilder<T>.Build(ServiceProviderMocker services)
        {
            if (_instance is null)
            {
                _instance = this.Build(services);
            }

            return _instance;
        }

        object IServiceBuilder.Build(ServiceProviderMocker services)
        {
            if (_instance is null)
            {
                _instance = this.Build(services);
            }

            return _instance;
        }

        protected abstract T Build(ServiceProviderMocker services);
    }
}
