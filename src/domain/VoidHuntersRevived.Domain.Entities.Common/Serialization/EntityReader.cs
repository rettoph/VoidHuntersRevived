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
        private readonly VhId _seed = seed;

        public int Position { get; private set; } = position;
        public readonly int Length => this._data.Length;
        public readonly bool DataAvailable => this.Position < this._data.Length;

        public EntityGlobalId ReadGlobalEntityId()
        {
            VhId raw = this.Read<VhId>();
            return this._seed.Create(raw).ToGlobalEntityId();
        }

        public byte ReadByte() => this._data[this.Position++];

        public bool ReadBoolean() => this.Read<bool>();

        public int ReadInt32() => this.Read<int>();

        public uint ReadUInt32() => this.Read<uint>();

        public unsafe T Read<T>()
            where T : unmanaged
        {
            fixed (byte* pByte = &this._data[this.Position])
            {
                this.Position += sizeof(T);

                T* pT = (T*)pByte;

                return pT[0];
            }
        }

        public void Skip(int bytes) => this.Position += bytes;
    }
}