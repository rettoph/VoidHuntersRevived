namespace VoidHuntersRevived.Tests.Common
{
    public interface IServiceBuilder
    {
        public object Build(ServiceProviderMocker services);
    }

    public interface IServiceBuilder<out T> : IServiceBuilder
        where T : class
    {
        new T Build(ServiceProviderMocker services);
    }
}
