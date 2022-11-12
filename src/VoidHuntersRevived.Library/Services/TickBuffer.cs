using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library
{
    internal class TickBuffer
    {
        public const int Length = 256;

        private Tick[] _buffer;

        private int _nextId;

        public int NextId
        {
            get => _nextId;
            set => _nextId = value;
        }

        public int LastEnqueuedId { get; private set; }

        public TickBuffer()
        {
            _nextId = Tick.MinimumValidId;
            _buffer = new Tick[TickBuffer.Length];

            Array.Fill(_buffer, Tick.Default);
        }

        public bool TryPop(out Tick tick)
        {
            if(this.TryGet(_nextId, out tick))
            {
                _nextId++;
                return true;
            }

            return false;
        }

        public bool TryGet(int id, out Tick tick)
        {
            int index = _nextId % TickBuffer.Length;
            tick = _buffer[index];

            if (tick.Id == id)
            {
                return true;
            }

            return false;
        }

        public void Enqueue(Tick tick)
        {
            int index = tick.Id % TickBuffer.Length;

            if(this.TryEnqueue(ref _buffer[index], tick))
            {
                this.LastEnqueuedId = tick.Id;
                return;
            }

            throw new NotImplementedException();
        }

        private bool TryEnqueue(ref Tick reference, Tick tick)
        {
            if(reference.Id == tick.Id)
            { // TODO: Verify this only happens on duplicate data
                return true;
            }

            if(reference.Id >= _nextId)
            {
                return false;
            }

            if(tick.Id > _nextId + TickBuffer.Length)
            {
                return false;
            }

            reference = tick;
            return true;
        }

        public int DecompressId(byte compressed)
        {
            var offset = this.NextId / byte.MaxValue * byte.MaxValue;

            return offset + compressed;
        }

        public byte CompressId(int id)
        {
            return (byte)(id % byte.MaxValue);
        }
    }
}
