namespace VoidHuntersRevived.Tests.Common
{
    public interface IInstanceBuilder<TOut>
        where TOut : notnull
    {
        TOut Build();

        TOut GetInstance();

        Lazy<TOut> GetLazy();

        Lazy<TLazy> GetLazy<TLazy>()
            where TLazy : class;
    }

    public interface IInstanceBuilder<TArg, TOut>
        where TOut : notnull
    {
        TOut Build(TArg arg);

        TOut GetInstance();

        Lazy<TOut> GetLazy();

        Lazy<TLazy> GetLazy<TLazy>()
            where TLazy : class;
    }
}
