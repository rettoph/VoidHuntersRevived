
using Guppy.Core.Common;

namespace VoidHuntersRevived.Tests.Common
{
    public abstract class BaseInstanceBuilder<TOut> : IInstanceBuilder<TOut>
        where TOut : notnull
    {
        private TOut? _instance;

        protected abstract TOut build();

        public TOut Build()
        {
            if (this._instance is not null)
            {
                throw new InvalidOperationException();
            }

            this._instance = this.build();
            return this._instance;
        }

        public TOut GetInstance()
        {
            return this._instance ?? this.Build();
        }

        public Lazy<TOut> GetLazy()
        {
            return new Lazy<TOut>(this.GetInstance);
        }

        public Lazy<TLazy> GetLazy<TLazy>()
            where TLazy : class
        {
            ThrowIf.Type.IsNotAssignableFrom<TLazy>(typeof(TLazy));

            return new Lazy<TLazy>(() => this.GetInstance().As<TLazy>());
        }
    }

    public abstract class BaseInstanceBuilder<TArg, TOut> : IInstanceBuilder<TArg, TOut>
    where TOut : notnull
    {
        private TOut? _instance;

        protected abstract TOut build(TArg arg);

        public TOut Build(TArg arg)
        {
            if (this._instance is not null)
            {
                throw new InvalidOperationException();
            }

            this._instance = this.build(arg);
            return this._instance;
        }

        public TOut GetInstance()
        {
            return this._instance ?? throw new NotImplementedException();
        }

        public Lazy<TOut> GetLazy()
        {
            return new Lazy<TOut>(this.GetInstance);
        }

        public Lazy<TLazy> GetLazy<TLazy>()
            where TLazy : class
        {
            ThrowIf.Type.IsNotAssignableFrom<TLazy>(typeof(TLazy));

            return new Lazy<TLazy>(() => this.GetInstance().As<TLazy>());
        }
    }
}