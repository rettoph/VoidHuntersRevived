using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Domain.Common.Providers
{
    public abstract class ServiceProvider<TKey, TService>
        where TKey : notnull
    {
        private readonly Dictionary<TKey, TService> _cache;

        public ServiceProvider()
        {
            this._cache = [];
        }

        protected abstract TService Factory(TKey key);

        public TService Get(TKey key)
        {
            ref TService? instance = ref CollectionsMarshal.GetValueRefOrAddDefault(this._cache, key, out bool exists);

            if (!exists)
            {
                instance = this.Factory(key);
            }

            return instance!;
        }

        public TService? Remove(TKey key)
        {
            if (this._cache.Remove(key, out TService? instance))
            {
                return instance;
            }

            return default;
        }
    }
}