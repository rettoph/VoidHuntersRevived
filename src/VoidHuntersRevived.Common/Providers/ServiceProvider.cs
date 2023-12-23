using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Common.Providers
{
    public abstract class ServiceProvider<TKey, TService>
        where TKey : notnull
    {
        private readonly Dictionary<TKey, TService> _cache;

        public ServiceProvider()
        {
            _cache = new Dictionary<TKey, TService>();
        }

        protected abstract TService Factory(TKey key);

        public TService Get(TKey key)
        {
            ref TService? instance = ref CollectionsMarshal.GetValueRefOrAddDefault(_cache, key, out bool exists);

            if (!exists)
            {
                instance = this.Factory(key);
            }

            return instance!;
        }

        public TService? Remove(TKey key)
        {
            if (_cache.Remove(key, out TService? instance))
            {
                return instance;
            }

            return default;
        }
    }
}
