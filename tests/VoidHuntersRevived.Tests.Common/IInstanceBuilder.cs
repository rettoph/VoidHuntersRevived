namespace VoidHuntersRevived.Tests.Common
{
    public interface IInstanceBuilder<TOut>
        where TOut : notnull
    {
        TOut BuildInstance();

        TOut GetInstance();

        Lazy<TOut> GetLazy();

        Lazy<TLazy> GetLazy<TLazy>()
            where TLazy : class;
    }

    public interface IInstanceBuilder<TArg, TOut>
        where TOut : notnull
    {
        TOut BuildInstance(TArg arg);

        TOut GetInstance();

        Lazy<TOut> GetLazy();

        Lazy<TLazy> GetLazy<TLazy>()
            where TLazy : class;
    }
}
