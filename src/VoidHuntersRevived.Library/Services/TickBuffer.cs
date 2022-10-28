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
        public const uint Length = 256;

        private Tick[] _buffer;

        private uint _nextId;

        public uint NextId
        {
            get => _nextId;
            set => _nextId = value;
        }

        public TickBuffer()
        {
            _nextId = Tick.MinimumValidId;
            _buffer = new Tick[TickBuffer.Length];

            Array.Fill(_buffer, Tick.Default);
        }

        public bool TryPop(out Tick tick)
        {
            uint index = _nextId % TickBuffer.Length;
            tick = _buffer[index];

            if(tick.Id == _nextId)
            {
                _nextId++;
                return true;
            }

            return false;
        }

        public void Enqueue(Tick tick)
        {
            uint index = tick.Id % TickBuffer.Length;

            if(this.TryEnqueue(ref _buffer[index], tick))
            {
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

        public uint DecompressId(byte compressed)
        {
            var offset = this.NextId / byte.MaxValue * byte.MaxValue;

            return offset + compressed;
        }

        public byte CompressId(uint id)
        {
            return (byte)(id % byte.MaxValue);
        }
    }
}
