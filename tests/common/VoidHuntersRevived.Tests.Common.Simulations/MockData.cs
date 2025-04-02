using VoidHuntersRevived.Common;
using VoidHuntersRevived.Tests.Common.Providers;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public class MockData<T>
        where T : new()
    {
        private readonly VhId _seed = VhId.NewVhId();
        private readonly Dictionary<object, T> _data = [];
        private readonly Dictionary<object, VhIdProvider> _vhids = [];

        public T GetDataByContext(object context)
        {
            if (this._data.TryGetValue(context, out T? value))
            {
                return value;
            }

            value = new();
            this._data.Add(context, value);
            return value;
        }

        public VhIdProvider GetVhIdsByContext(object context)
        {
            if (this._vhids.TryGetValue(context, out VhIdProvider? value))
            {
                return value;
            }

            value = new VhIdProvider(this._seed);
            this._vhids.Add(context, value);
            return value;
        }
    }
}
