using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Tests.Common.Providers
{
    public class VhIdProvider(VhId seed)
    {
        private int _noise = 0;
        private readonly VhId _seed = seed;

        public VhId Next() => this._seed.Create(this._noise++);
    }
}