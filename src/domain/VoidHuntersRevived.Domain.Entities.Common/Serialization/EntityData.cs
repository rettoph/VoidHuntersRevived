using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Serialization
{
    public readonly struct EntityData(VhId id, byte[] data, int[] indices)
    {
        private readonly byte[] _data = data;
        private readonly int[] _indices = indices;

        public readonly VhId Id = id;
        public int IndexCount => this._indices.Length;
        public int Length => this._data.Length;

        public EntityReader GetReader(VhId seed, int index, int offset = 0)
        {
            return new(seed, this._data, this._indices[index] + offset);
        }
    }
}