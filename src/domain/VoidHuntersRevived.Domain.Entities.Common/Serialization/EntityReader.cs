using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;

namespace VoidHuntersRevived.Domain.Entities.Common.Serialization
{
    public ref struct EntityReader
    {
        private readonly byte[] _data;
        private int _position;
        private readonly VhId _seed;

        public int Position => _position;
        public int Length => _data.Length;
        public bool DataAvailable => _position < _data.Length;

        public EntityReader(
            VhId seed,
            byte[] data,
            int position)
        {
            _seed = seed;
            _data = data;
            _position = position;
        }

        public GlobalEntityId ReadGlobalEntityId()
        {
            VhId raw = this.Read<VhId>();
            return _seed.Create(raw).ToGlobalEntityId();
        }

        public byte ReadByte()
        {
            return _data[_position++];
        }

        public bool ReadBoolean()
        {
            return this.Read<bool>();
        }

        public int ReadInt32()
        {
            return this.Read<int>();
        }

        public uint ReadUInt32()
        {
            return this.Read<uint>();
        }

        public unsafe T Read<T>()
            where T : unmanaged
        {
            fixed (byte* pByte = &_data[_position])
            {
                _position += sizeof(T);

                T* pT = (T*)pByte;

                return pT[0];
            }
        }

        public void Skip(int bytes)
        {
            _position += bytes;
        }
    }
}
