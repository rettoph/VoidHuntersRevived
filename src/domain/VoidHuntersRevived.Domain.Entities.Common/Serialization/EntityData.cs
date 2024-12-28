using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Serialization
{
    public readonly struct EntityData
    {
        private readonly byte[] _data;
        private readonly int[] _indices;

        public readonly VhId Id;
        public int IndexCount => _indices.Length;
        public int Length => _data.Length;

        public EntityData(VhId id, byte[] data, int[] indices)
        {
            _data = data;
            _indices = indices;

            this.Id = id;
        }

        public EntityReader GetReader(VhId seed, int index, int offset = 0)
        {
            return new EntityReader(seed, _data, _indices[index] + offset);
        }
    }
}
