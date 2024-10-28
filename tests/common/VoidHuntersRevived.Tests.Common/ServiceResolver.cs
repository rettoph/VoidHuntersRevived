namespace VoidHuntersRevived.Tests.Common
{
    internal interface IServiceResolver
    {
        Type Type { get; }
    }

    internal interface IServiceResolver<out T> : IServiceResolver
        where T : class
    {
        T GetInstance(ServiceProviderMocker services);
    }
}
