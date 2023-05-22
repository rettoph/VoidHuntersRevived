using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Lockstep.Providers;
using VoidHuntersRevived.Domain.Simulations.Utilities;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Providers
{
    internal class DefaultTickProvider : ITickProvider
    {
        private readonly TickBuffer _buffer;
        protected int nextId;

        public DefaultTickProvider()
        {
            _buffer = new TickBuffer();
        }

        public void Enqueue(Tick tick)
        {
            _buffer.Enqueue(tick);
        }

        public virtual bool TryDequeueNext([MaybeNullWhen(false)] out Tick tick)
        {
            if (_buffer.TryPop(this.nextId, out tick))
            {
                this.nextId++;
                return true;
            }

            return false;
        }

        public bool PeekHead([MaybeNullWhen(false)] out Tick tick)
        {
            if (_buffer.Head is null)
            {
                tick = null;
                return false;
            }

            tick = _buffer.Head;
            return true;
        }

        public bool PeekTail([MaybeNullWhen(false)] out Tick tick)
        {
            if (_buffer.Tail is null)
            {
                tick = null;
                return false;
            }

            tick = _buffer.Tail;
            return true;
        }

        public void Reset()
        {
            this.nextId = 0;
            _buffer.Clear();
        }
    }
}
