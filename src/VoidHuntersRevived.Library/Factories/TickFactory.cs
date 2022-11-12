using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Factories
{
    internal sealed class TickFactory : ITickFactory
    {
        private IList<ITickData> _datum;

        public TickFactory()
        {
            _datum = new List<ITickData>();
        }

        public void Enqueue(ITickData data)
        {
            _datum.Add(data);
        }

        public Tick Create(int id)
        {
            if(_datum.Count == 0)
            {
                return new Tick(id, Enumerable.Empty<ITickData>());
            }

            var tick = new Tick(id, _datum);
            _datum = new List<ITickData>();

            return tick;
        }
    }
}
