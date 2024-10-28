namespace VoidHuntersRevived.Tests.Common
{
    public abstract class ServiceBuilder<T> : IServiceBuilder<T>
        where T : class
    {
        T IServiceBuilder<T>.Build(ServiceProviderMocker services)
        {
            return this.Build(services);
        }

        object IServiceBuilder.Build(ServiceProviderMocker services)
        {
            return this.Build(services);
        }

        protected abstract T Build(ServiceProviderMocker services);
    }
}
