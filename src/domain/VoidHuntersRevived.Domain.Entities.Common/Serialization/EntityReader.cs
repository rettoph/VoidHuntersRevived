using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;

namespace VoidHuntersRevived.Domain.Entities.Common.Serialization
{
    public ref struct EntityReader(
        VhId seed,
        byte[] data,
        int position)
    {
        private readonly byte[] _data = data;
        private int _position = position;
        private readonly VhId _seed = seed;

        public readonly int Position => this._position;
        public readonly int Length => this._data.Length;
        public readonly bool DataAvailable => this._position < this._data.Length;

        public EntityGlobalId ReadGlobalEntityId()
        {
            VhId raw = this.Read<VhId>();
            return this._seed.Create(raw).ToGlobalEntityId();
        }

        public byte ReadByte()
        {
            return this._data[this._position++];
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
            fixed (byte* pByte = &this._data[this._position])
            {
                this._position += sizeof(T);

                T* pT = (T*)pByte;

                return pT[0];
            }
        }

        public void Skip(int bytes)
        {
            this._position += bytes;
        }
    }
}