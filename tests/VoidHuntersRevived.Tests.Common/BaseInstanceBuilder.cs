
using Guppy.Core.Common;
using Guppy.Tests.Common.Extensions;

namespace VoidHuntersRevived.Tests.Common
{
    public abstract class BaseInstanceBuilder<TOut> : IInstanceBuilder<TOut>
        where TOut : notnull
    {
        private TOut? _instance;

        protected abstract TOut build();

        public TOut BuildInstance()
        {
            if (_instance is not null)
            {
                throw new InvalidOperationException();
            }

            _instance = this.build();
            return _instance;
        }

        public TOut GetInstance()
        {
            return _instance ?? this.BuildInstance();
        }

        public Lazy<TOut> GetLazy()
        {
            return this.GetInstance().ToLazy();
        }

        public Lazy<TLazy> GetLazy<TLazy>()
            where TLazy : class
        {
            ThrowIf.Type.IsNotAssignableFrom<TLazy>(typeof(TLazy));

            return this.GetInstance().As<TLazy>().ToLazy();
        }
    }

    public abstract class BaseInstanceBuilder<TArg, TOut> : IInstanceBuilder<TArg, TOut>
    where TOut : notnull
    {
        private TOut? _instance;

        protected abstract TOut build(TArg arg);

        public TOut BuildInstance(TArg arg)
        {
            if (_instance is not null)
            {
                throw new InvalidOperationException();
            }

            _instance = this.build(arg);
            return _instance;
        }

        public TOut GetInstance()
        {
            return _instance ?? throw new NotImplementedException();
        }

        public Lazy<TOut> GetLazy()
        {
            return new Lazy<TOut>(() => this.GetInstance());
        }

        public Lazy<TLazy> GetLazy<TLazy>()
            where TLazy : class
        {
            ThrowIf.Type.IsNotAssignableFrom<TLazy>(typeof(TLazy));

            return new Lazy<TLazy>(() => this.GetInstance().As<TLazy>());
        }
    }
}
